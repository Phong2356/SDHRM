using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Models;
using SDHRM.Models.Constants;
using System.Security.Claims;

namespace SDHRM.Areas.Systems.Controllers
{
    [Area("Systems")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // 1. DANH SÁCH ROLE
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // 2. THÊM ROLE MỚI
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    TempData["SuccessMessage"] = "Thêm chức danh (Role) thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Chức danh này đã tồn tại!";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // 3. HIỂN THỊ TRANG CẤP QUYỀN (CLAIMS) CHO 1 ROLE
        public async Task<IActionResult> ManageClaims(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GenerateAllPermissions(); // Lấy danh sách 9 quyền khai báo ở Bước 1

            var model = new ManageRoleClaimsViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                Claims = new List<RoleClaimsViewModel>()
            };

            // Quét xem Role này đã được đánh dấu tích ở những quyền nào
            foreach (var permission in allPermissions)
            {
                model.Claims.Add(new RoleClaimsViewModel
                {
                    ClaimType = "Permission",
                    ClaimValue = permission,
                    IsSelected = existingClaims.Any(c => c.Type == "Permission" && c.Value == permission)
                });
            }

            return View(model);
        }

        // 4. LƯU LẠI CÁC QUYỀN (CLAIMS) VÀO DATABASE
        [HttpPost]
        public async Task<IActionResult> ManageClaims(ManageRoleClaimsViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null) return NotFound();

            // Xóa sạch các quyền (Claims) cũ của Role này
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Lưu lại những quyền (Claims) mới vừa được người dùng đánh dấu tích
            var selectedClaims = model.Claims.Where(c => c.IsSelected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.ClaimValue));
            }

            TempData["SuccessMessage"] = $"Cập nhật quyền cho {role.Name} thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}