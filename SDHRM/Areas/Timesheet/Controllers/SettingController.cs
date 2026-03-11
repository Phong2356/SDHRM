using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Timesheet.Controllers
{
    [Area("Timesheet")]
    [Authorize(Policy = "Timesheet.Manage")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetClientIpAddress()
        {
            var forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
                return forwardedHeader.Split(',')[0].Trim();

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        }

        // 1. DANH SÁCH WIFI (READ)
        public async Task<IActionResult> Index()
        {
            var data = await _context.CauHinhWifis.OrderByDescending(x => x.Id).ToListAsync();
            return View(data);
        }

        // 2. FORM THÊM MỚI (CREATE - GET)
        public IActionResult Create()
        {
            // Tự động lấy IP của Admin truyền ra View để gợi ý
            ViewBag.CurrentIP = GetClientIpAddress();
            return View();
        }

        // 3. XỬ LÝ LƯU THÊM MỚI (CREATE - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CauHinhWifi model)
        {
            if (ModelState.IsValid)
            {
                _context.CauHinhWifis.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm cấu hình WiFi thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CurrentIP = GetClientIpAddress();
            return View(model);
        }

        // 4. FORM SỬA (EDIT - GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cauHinh = await _context.CauHinhWifis.FindAsync(id);
            if (cauHinh == null) return NotFound();

            ViewBag.CurrentIP = GetClientIpAddress();
            return View(cauHinh);
        }

        // 5. XỬ LÝ LƯU SỬA (EDIT - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CauHinhWifi model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật cấu hình thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 6. XỬ LÝ XÓA (DELETE - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cauHinh = await _context.CauHinhWifis.FindAsync(id);
            if (cauHinh != null)
            {
                _context.CauHinhWifis.Remove(cauHinh);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa cấu hình WiFi!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

