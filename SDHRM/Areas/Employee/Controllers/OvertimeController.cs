using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SDHRM.Models;
using SDHRM.Data;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class OvertimeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OvertimeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Xem danh sách đơn
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            var danhSachDon = await _context.DonTangCas
                .Include(d => d.NguoiDuyet)
                .Where(d => d.NhanSuId == nhanSu.Id)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return View(danhSachDon);
        }

        // 2. Mở form Tạo mới
        [HttpGet]
        public IActionResult Create()
        {
            var model = new DonTangCa
            {
                NgayTangCa = DateTime.Today,
                TuGio = new TimeSpan(17, 30, 0), // Mặc định gợi ý 17:30
                DenGio = new TimeSpan(20, 30, 0), // Mặc định gợi ý 20:30
                SoGio = 3,
                LyDoTangCa = string.Empty
            };
            return View(model);
        }

        // 3. Xử lý lưu Tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonTangCa model)
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            ModelState.Remove("NhanSu");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                model.NhanSuId = nhanSu.Id;
                model.TrangThai = "Chờ duyệt";
                model.NgayTao = DateTime.Now;

                _context.DonTangCas.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã gửi đơn đăng ký tăng ca thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ==========================================
        // 4. MỞ FORM SỬA ĐƠN TĂNG CA (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            var don = await _context.DonTangCas
                .FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);

            if (don == null) return NotFound("Không tìm thấy đơn!");

            if (don.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] = "Đơn đã được xử lý, bạn không thể chỉnh sửa!";
                return RedirectToAction(nameof(Index));
            }

            return View(don);
        }

        // ==========================================
        // 5. LƯU THÔNG TIN SỬA ĐƠN (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonTangCa model)
        {
            if (id != model.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            var donGoc = await _context.DonTangCas.FindAsync(id);
            if (donGoc == null || donGoc.NhanSuId != nhanSu.Id) return NotFound();

            if (donGoc.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] = "Lỗi bảo mật: Đơn đã xử lý không thể lưu thay đổi!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("NhanSu");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                donGoc.NgayTangCa = model.NgayTangCa;
                donGoc.TuGio = model.TuGio;
                donGoc.DenGio = model.DenGio;
                donGoc.SoGio = model.SoGio;
                donGoc.LyDoTangCa = model.LyDoTangCa;

                _context.Update(donGoc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật đăng ký tăng ca thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 4. API Xóa đơn bằng AJAX
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

                var don = await _context.DonTangCas
                    .FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);

                if (don == null) return Json(new { success = false, message = "Không tìm thấy đơn!" });

                if (don.TrangThai != "Chờ duyệt")
                    return Json(new { success = false, message = "Chỉ có thể xóa đơn đang ở trạng thái Chờ duyệt!" });

                _context.DonTangCas.Remove(don);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa đơn đăng ký tăng ca!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}