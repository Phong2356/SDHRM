using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data; // <-- Thêm dòng này để gọi DbContext
using SDHRM.Models;
using SDHRM.Models.ViewModels;

namespace SDHRM.Areas.Systems.Controllers
{
    [Area("Systems")]
    [Authorize(Policy = "Systems.Manage")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context; 

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context; 
        }

        // 1. DANH SÁCH TÀI KHOẢN
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var nhanSus = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ViTriCongViec)
                .ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var nhanvien = nhanSus.FirstOrDefault(n => n.UserId == user.Id || n.Email == user.Email);

                var thisViewModel = new UserViewModel
                {
                    UserId = user.Id,
                    MaNV = nhanvien.Id,
                    Email = user.Email,
                    HoTen = nhanvien?.HoTen ?? "Chưa liên kết hồ sơ",
                    PhongBan = nhanvien?.PhongBan?.TenPhongBan ?? "Chưa phân bổ",
                    ViTriCongViec = nhanvien?.ViTriCongViec?.TenViTri ?? "Chưa thiết lập",
                    Roles = await _userManager.GetRolesAsync(user)
                };
                userViewModels.Add(thisViewModel);
            }

            return View(userViewModels);
        }
        // 2. MỞ TRANG CHỌN CHỨC DANH 
        public async Task<IActionResult> Manage(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var nhanvien = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == user.Id || n.Email == user.Email);

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                HoTen = nhanvien?.HoTen ?? "Tài khoản hệ thống",
                UserRoles = new List<UserRolesViewModel>()
            };

            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleName = role.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                };
                model.UserRoles.Add(userRolesViewModel);
            }

            return View(model);
        }

        // 3. LƯU LẠI CHỨC DANH ĐƯỢC CHỌN
        [HttpPost]
        public async Task<IActionResult> Manage(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            var selectedRoles = model.UserRoles.Where(x => x.IsSelected).Select(y => y.RoleName);
            await _userManager.AddToRolesAsync(user, selectedRoles);

            TempData["SuccessMessage"] = $"Đã cập nhật phân quyền cho tài khoản {model.Email}!";
            return RedirectToAction(nameof(Index));
        }
    }
}