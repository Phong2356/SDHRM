using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Controllers
{
    [Area("Employee")]
    [Authorize]
    public class BenefitController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BenefitController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<int?> GetCurrentNhanSuId()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);
            return nhanSu?.Id;
        }

        // 1. TRANG DANH SÁCH PHÚC LỢI
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các chương trình đang diễn ra hoặc sắp diễn ra
            var danhSachPhucLoi = await _context.PhucLois
                .Where(p => p.NgayKetThuc >= DateTime.Now.Date || p.TrangThai == "Đang thực hiện")
                .OrderByDescending(p => p.NgayBatDau)
                .ToListAsync();

            return View(danhSachPhucLoi);
        }

        // 2. TRANG CHI TIẾT
        public async Task<IActionResult> Details(int id)
        {
            var phucLoi = await _context.PhucLois.FirstOrDefaultAsync(p => p.Id == id);
            if (phucLoi == null) return NotFound();

            var nhanSuId = await GetCurrentNhanSuId();

            bool isDaThamGia = false;
            if (nhanSuId != null)
            {
                isDaThamGia = await _context.PhucLoiNhanViens
                    .AnyAsync(p => p.PhucLoiId == id && p.NhanSuId == nhanSuId);
            }

            ViewBag.IsDaThamGia = isDaThamGia;
            return View(phucLoi);
        }

        // 3. XỬ LÝ NÚT XÁC NHẬN THAM GIA / HỦY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleThamGia(int phucLoiId, string actionType)
        {
            var nhanSuId = await GetCurrentNhanSuId();
            if (nhanSuId == null) return BadRequest("Không tìm thấy hồ sơ nhân sự của bạn.");

            var existingRecord = await _context.PhucLoiNhanViens
                .FirstOrDefaultAsync(p => p.PhucLoiId == phucLoiId && p.NhanSuId == nhanSuId);

            if (actionType == "join" && existingRecord == null)
            {
                var dangKyMoi = new PhucLoiNhanVien
                {
                    PhucLoiId = phucLoiId,
                    NhanSuId = nhanSuId.Value,
                    GhiChu = "Nhân viên tự đăng ký qua Portal"
                };
                _context.PhucLoiNhanViens.Add(dangKyMoi);
            }
            else if (actionType == "leave" && existingRecord != null)
            {
                _context.PhucLoiNhanViens.Remove(existingRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = phucLoiId });
        }
    }
}