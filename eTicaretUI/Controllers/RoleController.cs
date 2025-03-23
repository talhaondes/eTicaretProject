using eTicaretData.Helpers;
using eTicaretData.Identity;
using eTicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretUI.Controllers
{

    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]

        public IActionResult Index()
        {
            if (_roleManager.Roles.ToList() == null)
            {
                return NotFound("Role bulunamadı");
            }
            return View(_roleManager.Roles.Where(i => i.Name != "Admin").ToList());
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Create() => View();
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel role)
        {
            var sonuc = await _roleManager.FindByNameAsync(role.Name);
            if (sonuc == null)
            {
                var result = await _roleManager.CreateAsync(new AppRole(role.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

            }
            return View(role);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rol = await _roleManager.FindByIdAsync(id.ToString());
            return View(rol);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model)
        {
            var getir = await _roleManager.FindByIdAsync(model.Id.ToString());
            getir.Name = model.Name;
            getir.NormalizedName = model.Name.ToUpper();
            var guncelle = await _roleManager.UpdateAsync(getir);
            if (guncelle.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(getir);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Remove(int id)
        {
            // Find the role by its ID
            var rol = await _roleManager.FindByIdAsync(id.ToString());

            // Check if the role exists
            if (rol != null)
            {
                // Remove the role from the system
                var result = await _roleManager.DeleteAsync(rol);

                // If the removal succeeded, redirect to the role list (Index action)
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            // If role not found or removal failed, return an error message
            return NotFound("Role not found or could not be removed.");
        }

    }
}
