using System.Reflection.Metadata;
using System.Xml.Linq;
using eTicaretDal.Abstract;
using eTicaretData.Entities;
using eTicaretData.Helpers;
using eTicaretData.Identity;
using eTicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUI.Controllers
{

    public class CartController : Controller
    {
        private readonly IProductDal _productDal;
        private readonly IOrderDal _orderDal;
        private readonly IAddressDal _addressDal;
        private readonly UserManager<AppUser> _userManager;

        public CartController(IProductDal productDal, IOrderDal orderDal, IAddressDal addressDal, UserManager<AppUser> userManager)
        {
            _productDal = productDal;
            _orderDal = orderDal;
            _addressDal = addressDal;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            if (cart == null || !cart.Any())
            {
                return View();
            }

            decimal total = cart.Sum(x => x.product.Price * x.Quantity);

            // Sepette en az 2 ürün varsa %20 indirim uygula
            if (cart.Count >= 2)
            {
                total *= 0.8m; // %20 indirim
            }

            ViewBag.Total = total.ToString("C");
            SessionHelper.Count = cart.Count;

            return View(cart);
        }
        public IActionResult Buy(int id)
        {
            if (SessionHelper.GetObjectFromJson<List<CartItem>>
                (HttpContext.Session, "cart") == null)
            {
                var cart = new List<CartItem>();
                cart.Add(new CartItem
                {
                    product = _productDal.GetById(id),
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                var cart = SessionHelper.GetObjectFromJson<List<CartItem>>
                    (HttpContext.Session, "cart");
                int index = isExist(cart, id);
                if (index < 0)
                {
                    cart.Add(new CartItem
                    {
                        product = _productDal.GetById(id),
                        Quantity = 1
                    });
                }
                else
                {
                    cart[index].Quantity++;
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        private int isExist(List<CartItem> cart, int id)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].product.ProductId.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        public IActionResult Remove(int id)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            if (cart != null)
            {
                var itemToRemove = cart.FirstOrDefault(x => x.product.ProductId == id);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }

                // Sepet artık boşsa, session'dan kaldırabilirsiniz
                if (cart.Count == 0)
                {
                    // Eğer varsa, SessionHelper.Remove("cart") metodu kullanılabilir.
                    // Alternatif olarak, cart'ı null olarak ayarlayıp, session'da boş liste olarak tutabilirsiniz.
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", null);
                }
            }
            return RedirectToAction("Index");
        }

        // GİRİŞ YAPMIŞ KULLANICI İÇİN CHECKOUT GET
        [HttpGet]
        public async Task<IActionResult> Checkout(int? addressId, string userName)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır.";
                return RedirectToAction("Index", "Cart");
            }

            // Giriş yapan kullanıcı
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                return RedirectToAction("Index", "Cart");
            }

            // Kullanıcının kayıtlı adreslerini al
            var userAddresses = _addressDal.GetAddressesByUserId(user.Id);
            // Adres yoksa, önce adres eklemesini iste
            if (!userAddresses.Any())
            {
                TempData["ErrorMessage"] = "Sipariş vermeden önce lütfen adres ekleyiniz.";
                return RedirectToAction("AddAddress", "Account");
            }

            // Dropdown list
            var addressList = userAddresses.Select(a => new SelectListItem
            {
                Value = a.AddressId.ToString(),
                Text = $"{a.AddressType} - {a.AddressLine1}, {a.District}, {a.City}"
            }).ToList();

            // Adres seçilmemişse ilk adresi default olarak seç
            if (!addressId.HasValue)
            {
                addressId = userAddresses.First().AddressId;
            }

            // ShippingDetail oluştur
            var shippingDetail = new ShippingDetail
            {
                UserName = !string.IsNullOrEmpty(userName) ? userName : user.UserName,
                Email = user.Email
            };

            // Seçilen adresin bilgilerini doldur
            var selectedAddress = userAddresses.FirstOrDefault(a => a.AddressId == addressId.Value);
            if (selectedAddress != null)
            {
                shippingDetail.AdressTitle = selectedAddress.AddressType;
                shippingDetail.AddressLine1 = selectedAddress.AddressLine1;
                shippingDetail.District = selectedAddress.District;
                shippingDetail.City = selectedAddress.City;
            }

            ViewBag.Total = cart.Sum(x => x.product.Price * x.Quantity).ToString("C");

            var model = new CheckoutViewModel
            {
                ShippingDetail = shippingDetail,
                Addresses = addressList,
                SelectedAddressId = addressId
            };

            return View("Checkout", model);
        }

        // GİRİŞ YAPMIŞ KULLANICI İÇİN CHECKOUT POST
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır.";
                return RedirectToAction("Index", "Cart");
            }

            // Kullanıcı bilgilerini al
            string userIdString = _userManager.GetUserId(User);
            if (!int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                return RedirectToAction("Index", "Cart");
            }

            var userAddresses = _addressDal.GetAddressesByUserId(userId);
            if (!userAddresses.Any())
            {
                TempData["ErrorMessage"] = "Sipariş vermeden önce lütfen adres ekleyiniz.";
                return RedirectToAction("AddAddress", "Account");
            }

            decimal total = cart.Sum(x => x.product.Price * x.Quantity);
            if (cart.Count >= 2)
            {
                total *= 0.8m;
            }
            ViewBag.Total = total.ToString("C");

            // Dropdown'dan adres seçimi yapılmadıysa
            if (!model.SelectedAddressId.HasValue)
            {
                TempData["ErrorMessage"] = "Lütfen bir adres seçiniz.";
                // Tekrar formu doldurup return View(model) diyebilirsiniz.
                var addressList = userAddresses.Select(a => new SelectListItem
                {
                    Value = a.AddressId.ToString(),
                    Text = $"{a.AddressType} - {a.AddressLine1}, {a.District}, {a.City}"
                }).ToList();
                model.Addresses = addressList;
                return View(model);
            }

            // Seçilen adresi bul
            var address = _addressDal.GetById(model.SelectedAddressId.Value);
            if (address == null)
            {
                TempData["ErrorMessage"] = "Seçtiğiniz adres bulunamadı.";
                var addressList = userAddresses.Select(a => new SelectListItem
                {
                    Value = a.AddressId.ToString(),
                    Text = $"{a.AddressType} - {a.AddressLine1}, {a.District}, {a.City}"
                }).ToList();
                model.Addresses = addressList;
                return View(model);
            }

            // BURADA Identity'den e-postayı kesin olarak alıyoruz
            var user = _userManager.GetUserAsync(User).Result; // veya await ile async
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                // Kullanıcının Identity tablosunda e-postası yoksa
                TempData["ErrorMessage"] = "Profilinizde bir e-posta adresi bulunamadı. Lütfen profilinizi güncelleyin.";
                return RedirectToAction("Index", "Cart");
            }

            // Artık e-postayı Identity'den zorla set ediyoruz
            var shippingDetails = new ShippingDetail
            {
                UserName = string.IsNullOrWhiteSpace(model.ShippingDetail.UserName)
                           ? User.Identity.Name
                           : model.ShippingDetail.UserName,
                AdressTitle = address.AddressType,
                AddressLine1 = address.AddressLine1,
                District = address.District,
                City = address.City,
                // Formdan gelen Email'i değil, Identity'deki Email'i kullanıyoruz çünkü üye olduysa bir daha email girdirmiyoruz
                Email = user.Email
            };

            int newOrderId = SaveOrder(cart, shippingDetails, shippingDetails.UserName, total);
            cart.Clear();
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            SessionHelper.Count = 0;
            return RedirectToAction("OrderCompleted", new { orderId = newOrderId });
        }



        [HttpGet]
        public IActionResult CheckoutAsGuest()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır.";
                return RedirectToAction("Index", "Cart");
            }
            ViewBag.Total = cart.Sum(x => x.product.Price * x.Quantity).ToString("C");

            // Checkout view'i CheckoutViewModel tipinde model beklediğinden, burada da böyle bir model oluşturuyoruz.
            var model = new CheckoutViewModel
            {
                ShippingDetail = new ShippingDetail(), // Boş form, misafir için
                Addresses = new List<SelectListItem>()   // Misafir kullanıcıların kayıtlı adresi yok
            };

            return View("CheckoutAsGuest", model);
        }



        [HttpPost]
        public IActionResult CheckoutAsGuest(CheckoutViewModel model)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır.";
                return RedirectToAction("Index", "Cart");
            }

            // Formda girilen ShippingDetail bilgisi burada: model.ShippingDetail
            // Misafir kullanıcıya özel UserName veriyoruz:
            model.ShippingDetail.UserName = "Misafir_" + Guid.NewGuid().ToString("N");

            decimal total = cart.Sum(x => x.product.Price * x.Quantity);
            if (cart.Count >= 2)
            {
                total *= 0.8m;
            }

            ViewBag.Total = total.ToString("C");

            // Siparişi kaydet
            int newOrderId = SaveOrder(cart, model.ShippingDetail, model.ShippingDetail.UserName, total);

            // Sepeti temizle
            cart.Clear();
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            SessionHelper.Count = 0;

            return RedirectToAction("OrderCompleted", new { orderId = newOrderId });
        }


        private int SaveOrder(List<CartItem> cart, ShippingDetail detail, string userName, decimal totalPrice)
        {
            var order = new Order
            {
                OrderNumber = Guid.NewGuid().ToString("N"),
                AdressTitle = detail.AdressTitle,
                AddressLine1 = detail.AddressLine1,
                District = detail.District,
                City = detail.City,
                TotalPrice = totalPrice,
                OrderDate = DateTime.Now,
                Username = userName,
                OrderState = EnumOrderState.Waiting,
                Email = detail.Email,
                OrderLines = new List<OrderLine>()
            };


            foreach (var item in cart)
            {
                var orderLine = new OrderLine
                {
                    ProductId = item.product.ProductId,
                    OrderLineQuantity = item.Quantity,
                    OrderLinePrice = item.Quantity * item.product.Price
                };
                order.OrderLines.Add(orderLine);

                var product = _productDal.GetById(item.product.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _productDal.update(product);
                }
            }

            _orderDal.add(order);
            return order.OrderId;

        }
        public IActionResult OrderCompleted(int orderId)
        {
            var order = _orderDal.GetOrderWithDetails(orderId);
            if (order == null) return NotFound("Sipariş Bulunamadı");

            return View(order);
        }

    }

}

