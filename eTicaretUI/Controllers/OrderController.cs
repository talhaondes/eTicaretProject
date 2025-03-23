using eTicaretDal.Abstract;
using eTicaretDal.Concrete;
using eTicaretData.Context;
using eTicaretData.Entities;
using eTicaretData.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly ETicaretContext _context;
        private readonly IOrderDal _orderDal;
        private readonly IOrderLineDal  _lineDal;

        public OrderController(ETicaretContext context, IOrderDal orderDal, IOrderLineDal lineDal)
        {
            _context = context;
            _orderDal = orderDal;
            _lineDal = lineDal;
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var algetir = _orderDal.GetById(id);
            if (algetir == null)
            {
                return NotFound(); 
            }
            return View(algetir);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var orderr = _orderDal.GetById(id);

            if (orderr == null)
            {
                return NotFound(); 
            }
            _orderDal.delete(orderr);
            return RedirectToAction("Index"); 
        }
        public IActionResult Index()
        {
            var getir = _orderDal.GetAll();
            return View(getir);
        }
        public IActionResult Detail(int id)
        {
            var order = _orderDal.GetOrderWithDetails(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [HttpGet]
        public IActionResult Complete(int id)
        {
            // 1) order'ı veritabanından bul
            var order = _orderDal.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            // 2) order durumunu Completed olarak ayarla
            order.OrderState = EnumOrderState.Completed;

            // 3) Güncelleme (veritabanına kaydetme) işlemini yap
            _orderDal.update(order);

            // 4) Tekrar siparişler sayfasına veya istediğiniz herhangi bir sayfaya yönlendirin
            return RedirectToAction("Index");
        }


    }
}
