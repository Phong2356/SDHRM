using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Authorize(Policy = "Payroll.Manage")]
    public class PayrolltemplateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrolltemplateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH MẪU BẢNG LƯƠNG
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.MauBangLuongs
                .Include(m => m.ChiTietMaus)
                .OrderByDescending(m => m.NgayTao)
                .ToListAsync();
            return View(danhSach);
        }

        // 2. THÊM MỚI NHANH 1 MẪU TỪ MODAL
        [HttpPost]
        public async Task<IActionResult> Create(string TenMau, string GhiChu)
        {
            var mauMoi = new MauBangLuong { TenMau = TenMau, GhiChu = GhiChu };
            _context.MauBangLuongs.Add(mauMoi);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm mẫu bảng lương thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 3. TRANG THIẾT LẬP CÁC TRƯỜNG THÀNH PHẦN LƯƠNG
        [HttpGet]
        public async Task<IActionResult> Config(int id)
        {
            var mau = await _context.MauBangLuongs
                .Include(m => m.ChiTietMaus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mau == null) return NotFound();

            // Kéo toàn bộ danh mục thành phần lương đã Kích hoạt để hiển thị ra Checkbox
            ViewBag.TatCaThanhPhan = await _context.ThanhPhanLuongs
                .Where(t => t.KichHoat)
                .OrderBy(t => t.TinhChat)
                .ThenBy(t => t.Id)
                .ToListAsync();

            return View(mau);
        }

        [HttpPost]
        public async Task<IActionResult> Config(int id, int[] selectedComponents)
        {
            var mau = await _context.MauBangLuongs
                .Include(m => m.ChiTietMaus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mau == null) return NotFound();

            // Xóa sạch cấu hình cũ của mẫu này
            _context.ChiTietMauBangLuongs.RemoveRange(mau.ChiTietMaus);

            // Thêm cấu hình mới dựa trên các checkbox người dùng vừa tick
            int thuTuHienThi = 1;
            if (selectedComponents != null)
            {
                foreach (var compId in selectedComponents)
                {
                    _context.ChiTietMauBangLuongs.Add(new ChiTietMauBangLuong
                    {
                        MauBangLuongId = id,
                        ThanhPhanLuongId = compId,
                        ThuTu = thuTuHienThi++
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã lưu thiết lập các trường thành phần cho mẫu lương!";
            return RedirectToAction(nameof(Index));
        }
    }
}
