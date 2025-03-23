using System.Diagnostics;
using eTicaretDal.Abstract;
using eTicaretDal.Concrete;
using eTicaretUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eTicaretUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryDal _categoryal;
        private readonly IProductDal _productDal;

        public HomeController(ILogger<HomeController> logger, ICategoryDal categoryDal, IProductDal productDal)
        {
            _categoryal = categoryDal;
            _productDal = productDal;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var urunler = _productDal.GetAll(p => p.IsHome && p.IsApproved);
            return View(urunler);
        }

        public IActionResult List(int? selecteditem)
        {
            var urunler = _productDal.GetAll(p => p.IsHome && p.IsApproved);

            if (selecteditem.HasValue)
            {
                urunler = urunler.Where(p => p.CategoryId == selecteditem.Value).ToList();
            }

            List<SelectListItem> kategoriler = _productDal.GetAll()
                .GroupBy(x => x.CategoryId)
                .Select(g => new SelectListItem
                {
                    Text = _categoryal.GetById(g.Key)?.CategoryName,
                    Value = g.Key.ToString()
                })
                .ToList();
            ViewBag.selecteditem = selecteditem;
            ViewBag.kategoriler = kategoriler;
            return View(urunler);
        }
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var urun = _productDal.GetById(id);
            if (urun == null)
            {
                return NotFound();
            }
            return View(urun);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
