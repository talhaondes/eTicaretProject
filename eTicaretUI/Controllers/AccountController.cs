using eTicaretData.Identity;
using eTicaretData.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using eTicaretData.Helpers;
using Microsoft.AspNetCore.Authentication;
using eTicaretData.Context;
using eTicaretData.Entities;
using eTicaretDal.Abstract;

namespace ETicaretUIKatmanı.Controllers
{
    [Authorize]  // Bu sınıfa giriş yapmamış bir kullanıcı erişemez.
    public class AccountController : Controller
    {
        private readonly ETicaretContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IAddressDal _addressDal;

        public AccountController(ETicaretContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IAddressDal addressDal)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _addressDal = addressDal;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]  // Kullanıcı girişi yapmamış olsa bile bu sayfayı ziyaret edebilir.
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kullanıcı adı ve şifre ile giriş denemesi yapılıyor.
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.PassWord, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Hatalı giriş durumunda genel bir hata ekleniyor.
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış.");
                return View(model);
            }
        }



        [HttpGet]
        [AllowAnonymous]  // Kullanıcı girişi yapmamış olsa bile bu sayfayı ziyaret edebilir.
        public IActionResult Register()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // Giriş yapmamış kullanıcılar erişebilir.
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Kullanıcı adı kontrolü
                var existingUserByName = await _userManager.FindByNameAsync(registerViewModel.UserName);
                if (existingUserByName != null)
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten kullanılıyor.");
                }

                // E-posta kontrolü
                var existingUserByEmail = await _userManager.FindByEmailAsync(registerViewModel.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                }

                // Herhangi biri mevcutsa, kullanıcıya hatayı göster.
                if (!ModelState.IsValid)
                {
                    return View(registerViewModel);
                }

                var user = new AppUser
                {
                    UserName = registerViewModel.UserName,
                    Email = registerViewModel.Email,
                    Name = registerViewModel.Name,
                    SurName = registerViewModel.SurName,
                    PhoneNumber = registerViewModel.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.PassWord);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(registerViewModel);
        }

        // Kullanıcıdan çıkış yapmayı onaylamak için GET isteği
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public IActionResult Logout()
        {
            return View();
        }

        // Çıkış yapmak için POST isteği
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> LogoutPost()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                cart.Clear();
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            // Tüm session verilerini temizleyin
            HttpContext.Session.Clear();
            // Statik sepet sayısını sıfırlayın
            SessionHelper.Count = 0;

            await _signInManager.SignOutAsync();
            Response.Cookies.Delete(".AspNetCore.Identity.Application");

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userId = User.Identity.Name; // Kullanıcının adı (veya email'i)
            var user = await _userManager.FindByNameAsync(userId); // Kullanıcıyı asenkron olarak bul

            if (user == null)
            {
                // Eğer kullanıcı bulunamazsa hata döndür veya yönlendir
                return NotFound();
            }
            var addresses = _addressDal.GetAddressesByUserId(user.Id).ToList();

            var orders = await _context.Orders
               .Where(o => o.Email == user.Email) // Kullanıcının UserName'ine göre filtrele
                .ToListAsync();
            // Admin kullanıcı için tüm siparişleri al
            List<Order> allOrders = new List<Order>();
            if (User.IsInRole("Admin"))
            {
                allOrders = await _context.Orders.ToListAsync(); // Admin için tüm siparişleri al
            }
            // ProfileViewModel nesnesini dolduralım
            var mmodel = new ProfileViewModel
            {
                Name = user.Name,
                SurName = user.SurName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                Orders = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,  // <-- Burada OrderId'yi atıyoruz
                    OrderNumber = o.OrderNumber,
                    TotalPrice = o.TotalPrice,
                    AdressTitle = o.AdressTitle,
                    City = o.City,
                    OrderDate = o.OrderDate,
                    OrderState = o.OrderState.ToString(), // Enum'ı string'e dönüştür
                }).ToList(),
                AllOrders = allOrders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,  // <-- Burada OrderId'yi atıyoruz
                    OrderNumber = o.OrderNumber,
                    TotalPrice = o.TotalPrice,
                    AdressTitle = o.AdressTitle,
                    City = o.City,
                    OrderDate = o.OrderDate,
                    OrderState = o.OrderState.ToString(),
                    UserName = o.Username

                }).ToList(),
                Addresses = addresses

            };

            // Eğer gelen modelde yeni veriler varsa, bunları veritabanına kaydedebiliriz
            if (ModelState.IsValid)
            {
                // Kullanıcı bilgilerini güncellemek için işlemler yapılabilir
                user.Name = model.Name;
                user.SurName = model.SurName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu.";
                }
            }

            // Güncellenmiş bilgileri ve siparişleri View'e gönder
            return View(mmodel);
        }
        [HttpGet]
        public IActionResult AddAddress()
        {
            return View(new AddAdressModel());
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress(AddAdressModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var addressEntity = new Address
            {
                AddressLine1 = model.AddressLine1,
                District = model.District,
                City = model.City,
                AddressType = model.AddressType,
                UserId = user.Id
            };

            _addressDal.add(addressEntity);
            TempData["SuccessMessage"] = "Adres başarıyla eklendi.";

            return RedirectToAction("Profile");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            // Silinecek adresi çekiyoruz
            var address = _addressDal.GetById(id);
            if (address == null)
            {
                TempData["ErrorMessage"] = "Adres bulunamadı.";
                return RedirectToAction("Profile");
            }

            // Giriş yapan kullanıcıyı alıyoruz
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                return RedirectToAction("Profile");
            }

            // Güvenlik kontrolü: Silinecek adresin kullanıcıya ait olduğunu doğrulayın.
            if (address.UserId != user.Id)
            {
                TempData["ErrorMessage"] = "Bu işlemi yapmaya yetkiniz yok.";
                return RedirectToAction("Profile");
            }

            // Adresi sil
            _addressDal.delete(address); // DAL'da Delete metodu tanımlı olmalı
            TempData["SuccessMessage"] = "Adres başarıyla silindi.";

            return RedirectToAction("Profile");
        }



    }
}
