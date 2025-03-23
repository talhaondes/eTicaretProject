using System.ComponentModel.DataAnnotations;
using eTicaretDal.Abstract;
using eTicaretDal.Concrete;
using eTicaretData.Context;
using eTicaretData.Entities;
using eTicaretData.Helpers;
using eTicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly ETicaretContext _context;
        private readonly IProductDal _productdal;
        private readonly ICategoryDal _categorydal;

        public ProductController(ETicaretContext context, IProductDal productdal, ICategoryDal categoryDal)
        {
            _categorydal = categoryDal;
            _context = context;
            _productdal = productdal;
        }
        
        // User ve Admin her ikisi de ürünleri listeleyebilir veya 
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            return View(_productdal.GetAll());
        }

        // Yalnızca Admin rolüne sahip kullanıcılar ürün ekleyebilir
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // Yalnızca Admin rolüne sahip kullanıcılar ürün ekleyebilir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId", "Name", "Description", "Image", "Stock", "Price", "IsApproved", "IsHome", "CategoryId")] Product product)
        {
            if (!ModelState.IsValid)
            {
                _productdal.add(product);   
                return RedirectToAction("Index", "Product");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // Yalnızca Admin rolüne sahip kullanıcılar ürün düzenleyebilir
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            var urun = _context.Products.FirstOrDefault(p => p.ProductId == id);
            return View(urun);
        }

        // Yalnızca Admin rolüne sahip kullanıcılar ürün düzenleyebilir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId", "Name", "Image", "Stock", "Price", "IsApproved", "IsHome", "CategoryId", "Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            _productdal.update(product);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return RedirectToAction("Index");
        }

        // Herkes ürün detaylarını görebilir
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var urun = _productdal.GetById(id);
            if (urun == null)
            {
                return NotFound();
            }
            return View(urun);
        }

        // Yalnızca Admin rolüne sahip kullanıcılar ürün silebilir
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var urun = await _context.Products.Include(p => p.category).FirstOrDefaultAsync(x => x.ProductId == id);
            if (urun == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(urun);
        }

        // Yalnızca Admin rolüne sahip kullanıcılar ürün silebilir
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(Product product)
        {
            _productdal.delete(product);
            return RedirectToAction("Index");
        }
    }
}
