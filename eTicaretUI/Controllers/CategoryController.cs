using eTicaretDal.Abstract;
using eTicaretDal.Concrete;
using eTicaretData.Context;
using eTicaretData.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eTicaretUI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ETicaretContext _context;
        private readonly ICategoryDal _categoryDal;

        public CategoryController(ETicaretContext context, ICategoryDal categoryDal)
        {
            _context = context;
            _categoryDal = categoryDal;
        }

        public IActionResult Index()
        {
            var sonuc = _categoryDal.GetAll();
            return View(sonuc);
        }
        // Kategori oluşturma (GET)

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        // Kategori oluşturma (post)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CategoryId", "Description", "CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryDal.add(category);
                return RedirectToAction("Index", "Category");
            }
            return View(category);
        }

        // Kategori düzenleme (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var category = _categoryDal.GetById(id); // Kategoriyi id'ye göre al
            if (category == null)
            {
                return NotFound(); // Kategori bulunamadıysa 404 döndür
            }
            return View(category); // Kategori düzenleme formunu göster
        }

        // Kategori düzenleme (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, [Bind("CategoryId", "Description", "CategoryName")] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest(); // Kategorinin id'si eşleşmiyorsa hatalı istek
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryDal.update(category); // Kategoriyi güncelle
                }
                catch (Exception)
                {
                    if (_categoryDal.GetById(id) == null)
                    {
                        return NotFound(); // Kategori bulunamazsa 404 döndür
                    }
                    else
                    {
                        throw; // Diğer hatalar için hata fırlat
                    }
                }
                return RedirectToAction(nameof(Index)); // Güncelledikten sonra kategori listesine yönlendir
            }
            return View(category); // Geçersiz model durumunda tekrar formu göster
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null)
            {
                return NotFound(); // Return 404 if category doesn't exist
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            _categoryDal.delete(category); // Delete the category from the database
            return RedirectToAction("Index"); // Redirect to the category list after deletion
        }
    }
}
