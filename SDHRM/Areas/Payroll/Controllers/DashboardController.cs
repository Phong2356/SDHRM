using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Authorize(Policy = "Payroll.View")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. TỔNG HỢP LƯƠNG (Lấy bảng lương mới nhất)
            var bangLuongMoiNhat = await _context.BangLuongs
                .OrderByDescending(b => b.NgayTao)
                .FirstOrDefaultAsync();

            decimal tongLuong = 0, thueTNCN = 0, baoHiem = 0;
            if (bangLuongMoiNhat != null)
            {
                var chiTiets = await _context.ChiTietBangLuongs
                    .Include(c => c.KetQuaLuongs).ThenInclude(k => k.ThanhPhanLuong)
                    .Where(c => c.BangLuongId == bangLuongMoiNhat.Id)
                    .ToListAsync();

                tongLuong = chiTiets.Sum(c => c.TongThuNhap);

                // Lọc tiền Thuế và Bảo hiểm từ dữ liệu động
                thueTNCN = chiTiets.SelectMany(c => c.KetQuaLuongs)
                    .Where(k => k.ThanhPhanLuong != null && k.ThanhPhanLuong.MaThanhPhan == "THUE_TNCN")
                    .Sum(k => k.SoTien);

                baoHiem = chiTiets.SelectMany(c => c.KetQuaLuongs)
                    .Where(k => k.ThanhPhanLuong != null && (k.ThanhPhanLuong.MaThanhPhan == "BHXH_NV" || k.ThanhPhanLuong.MaThanhPhan == "BHYT_NV" || k.ThanhPhanLuong.MaThanhPhan == "BHTN_NV"))
                    .Sum(k => k.SoTien);
            }

            ViewBag.BangLuongMoiNhat = bangLuongMoiNhat;
            ViewBag.TongLuong = tongLuong;
            ViewBag.ThueTNCN = thueTNCN;
            ViewBag.BaoHiem = baoHiem;

            // 2. PHÂN TÍCH MỨC LƯƠNG NHÂN VIÊN (Dựa vào Lương Cơ Bản)
            var hoSos = await _context.HoSoLuongs.ToListAsync();

            int duoi10 = hoSos.Count(h => h.LuongCoBan < 10000000);
            int tu10den20 = hoSos.Count(h => h.LuongCoBan >= 10000000 && h.LuongCoBan < 20000000);
            int tu20den30 = hoSos.Count(h => h.LuongCoBan >= 20000000 && h.LuongCoBan < 30000000);
            int tren30 = hoSos.Count(h => h.LuongCoBan >= 30000000);

            ViewBag.Duoi10 = duoi10;
            ViewBag.Tu10Den20 = tu10den20;
            ViewBag.Tu20Den30 = tu20den30;
            ViewBag.Tren30 = tren30;
            ViewBag.MaxCount = new[] { duoi10, tu10den20, tu20den30, tren30, 1 }.Max();

            return View();
        }
    }
}