using eTicaretData.Identity;
using eTicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUI.Controllers
{
    // Admin rolündeki kullanıcıların erişebileceği controller
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Adminlerin kullanıcı listesini görmesi
        public async Task<IActionResult> Index()
        {
            // Admin kullanıcıları veritabanından al
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            // Admin olmayan kullanıcıları filtrelemek için admin Id'lerini al
            var adminIds = admins.Select(a => a.Id).ToList();

            // Admin olmayan kullanıcıları filtrele ve veritabanından al
            var users = _userManager.Users
                                    .Where(i => !adminIds.Contains(i.Id)) // Admin Id'leri dışında kalanları al
                                    .ToList(); // Veritabanından asenkron olarak al

            return View(users);
        }

        // Adminin kullanıcıyı silmesi
        public async Task<IActionResult> Delete(int id)
        {
            // Kullanıcıyı bul
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                // Kullanıcı bulunamadıysa hata mesajı
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

            // Kullanıcıyı sil
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Başarılıysa kullanıcıyı silmiş ve başarılı mesajı vermiş olalım
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                // Hata varsa, hata mesajını gösterelim
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        // Kullanıcı rol ataması sayfası
        [HttpGet]
        public async Task<IActionResult> RoleAssign(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var roles = _roleManager.Roles.Where(i => i.Name != "Admin").ToList();
            var userroles = await _userManager.GetRolesAsync(user);
            var RoleAssing = new List<RoleAssignViewModel>();
            roles.ForEach(role => RoleAssing.Add(new RoleAssignViewModel
            {
                Id = role.Id,
                HasAssing = userroles.Contains(role.Name),
                Name = role.Name,
            }));
            ViewBag.name = user.Name;
            return View(RoleAssing);
        }

        // Kullanıcı rollerini güncelleme
        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> models, int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            // Kullanıcının mevcut rollerini al
            var userRoles = await _userManager.GetRolesAsync(user);

            // Kullanıcının rollerini güncelleme
            var selectedRoles = models.Where(x => x.HasAssing).Select(x => x.Name).ToList();

            // Mevcut olan rollerden kaldırılacakları belirleyelim
            var rolesToRemove = userRoles.Except(selectedRoles).ToList();
            var rolesToAdd = selectedRoles.Except(userRoles).ToList();

            // Rollerden çıkarma işlemi
            foreach (var role in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Yeni roller ekleme işlemi
            foreach (var role in rolesToAdd)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            // İşlem sonrası başarı mesajı
            TempData["SuccessMessage"] = "Roller başarıyla güncellendi.";

            // Güncellenmiş rollerle aynı sayfaya yönlendir
            return RedirectToAction("RoleAssign", new { id = user.Id });
        }
    }
}
