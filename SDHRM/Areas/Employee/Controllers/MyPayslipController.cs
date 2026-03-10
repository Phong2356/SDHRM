using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize]
    public class MyPayslipController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyPayslipController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hàm hỗ trợ lấy ID Nhân sự hiện tại
        private async Task<int?> GetCurrentNhanSuId()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);
            return nhanSu?.Id;
        }

        // 1. DANH SÁCH PHIẾU LƯƠNG CÁ NHÂN
        public async Task<IActionResult> Index()
        {
            var nhanSuId = await GetCurrentNhanSuId();
            if (nhanSuId == null) return NotFound("Không tìm thấy thông tin nhân sự.");

            // Chỉ lấy các phiếu lương đã được HR Gửi, Duyệt hoặc Chi trả
            var danhSach = await _context.ChiTietBangLuongs
                .Include(c => c.BangLuong)
                .Where(c => c.NhanSuId == nhanSuId &&
                            (c.BangLuong.TrangThai == "Đã gửi phiếu lương" ||
                             c.BangLuong.TrangThai == "Đã duyệt" ||
                             c.BangLuong.TrangThai == "Đã chi trả"))
                .OrderByDescending(c => c.BangLuong.NgayTao)
                .ToListAsync();

            return View(danhSach);
        }

        // 2. XEM CHI TIẾT PHIẾU LƯƠNG
        public async Task<IActionResult> Detail(int id)
        {
            var nhanSuId = await GetCurrentNhanSuId();

            var chiTiet = await _context.ChiTietBangLuongs
                .Include(c => c.BangLuong)
                .Include(c => c.NhanSu).ThenInclude(n => n.PhongBan)
                .Include(c => c.KetQuaLuongs).ThenInclude(k => k.ThanhPhanLuong)
                .FirstOrDefaultAsync(c => c.Id == id && c.NhanSuId == nhanSuId);

            if (chiTiet == null) return NotFound();

            return View(chiTiet);
        }

        // 3. XỬ LÝ NÚT XÁC NHẬN HOẶC GỬI THẮC MẮC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFeedback(int id, string actionType, string? thacMac)
        {
            var nhanSuId = await GetCurrentNhanSuId();
            var chiTiet = await _context.ChiTietBangLuongs.FirstOrDefaultAsync(c => c.Id == id && c.NhanSuId == nhanSuId);

            if (chiTiet == null) return NotFound();

            if (actionType == "confirm")
            {
                chiTiet.TrangThaiXacNhan = "Đã xác nhận";
                chiTiet.ThacMac = null; // Xóa thắc mắc cũ nếu có
                TempData["SuccessMessage"] = "Bạn đã xác nhận phiếu lương thành công!";
            }
            else if (actionType == "feedback")
            {
                if (string.IsNullOrWhiteSpace(thacMac))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập nội dung thắc mắc.";
                    return RedirectToAction(nameof(Detail), new { id = id });
                }
                chiTiet.TrangThaiXacNhan = "Có thắc mắc";
                chiTiet.ThacMac = thacMac;
                TempData["SuccessMessage"] = "Đã gửi thắc mắc đến bộ phận Nhân sự/Kế toán.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}