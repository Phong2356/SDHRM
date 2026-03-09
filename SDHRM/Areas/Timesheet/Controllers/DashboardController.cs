using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using SDHRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDHRM.Areas.Timesheet.Controllers
{
    [Area("Timesheet")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // HÀM LÕI TRUY VẤN DỮ LIỆU CHẤM CÔNG (DÙNG CHUNG)
        // ==========================================
        private IQueryable<ChiTietChamCongNgay> GetBaseAttendanceQuery(DateTime startDate, DateTime endDate, string phongBan)
        {
            var query = _context.ChiTietChamCongNgays
                .Include(c => c.BangChamCongNhanVien)
                    .ThenInclude(b => b.NhanSu)
                        .ThenInclude(n => n.PhongBan)
                .Include(c => c.BangChamCongNhanVien)
                    .ThenInclude(b => b.NhanSu)
                        .ThenInclude(n => n.ViTriCongViec)
                .Where(c => c.Ngay.Date >= startDate.Date && c.Ngay.Date <= endDate.Date)
                .AsQueryable();

            if (!string.IsNullOrEmpty(phongBan))
                query = query.Where(c => c.BangChamCongNhanVien.NhanSu.PhongBan.TenPhongBan == phongBan);

            return query;
        }

        // ==========================================
        // 1. TRANG CHỦ DASHBOARD (GIAO DIỆN & 3 THẺ TỔNG QUAN)
        // ==========================================
        public async Task<IActionResult> Dashboard()
        {
            var vm = new DashboardChamCongVM();
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            // Thẻ Cam: Đi muộn về sớm (Hôm nay)
            vm.TongDiMuonVeSom = await _context.ChiTietChamCongNgays
                .Where(c => c.Ngay.Date == today && (c.PhutDiMuon > 0 || c.PhutVeSom > 0))
                .CountAsync();

            // Thẻ Xanh dương: Thực tế nghỉ (Hôm nay)
            vm.TongThucTeNghi = await _context.DonXinNghis
                .Where(d => d.TrangThai == "Đã duyệt" && d.TuNgay.Date <= today && d.DenNgay.Date >= today)
                .Select(d => d.NhanSuId).Distinct().CountAsync();

            // Thẻ Xanh lá: Kế hoạch nghỉ (Ngày mai)
            vm.TongKeHoachNghi = await _context.DonXinNghis
                .Where(d => d.TrangThai == "Đã duyệt" && d.TuNgay.Date <= tomorrow && d.DenNgay.Date >= tomorrow)
                .Select(d => d.NhanSuId).Distinct().CountAsync();

            return View(vm);
        }

        // ==========================================
        // 2. API BIỂU ĐỒ LINE: ĐI MUỘN THEO THỜI GIAN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetApiDiMuonTheoThoiGian(DateTime? tuNgay, DateTime? denNgay, string phongBan = "")
        {
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            DateTime startDate = tuNgay ?? endDate.AddDays(-30);

            var rawData = await GetBaseAttendanceQuery(startDate, endDate, phongBan)
                .Where(c => c.PhutDiMuon > 0 || c.PhutVeSom > 0)
                .GroupBy(c => c.Ngay.Date)
                .Select(g => new { Ngay = g.Key, SoLan = g.Count() })
                .ToListAsync();

            var labels = new List<string>();
            var dataValues = new List<int>();

            for (var dt = startDate.Date; dt <= endDate.Date; dt = dt.AddDays(1))
            {
                labels.Add(dt.ToString("dd/MM"));
                var record = rawData.FirstOrDefault(x => x.Ngay == dt);
                dataValues.Add(record != null ? record.SoLan : 0);
            }

            return Json(new { success = true, labels = labels, data = dataValues });
        }

        // ==========================================
        // 3. API BIỂU ĐỒ CỘT: TÌNH HÌNH NGHỈ THEO PHÒNG BAN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetApiNghiTheoPhongBan(DateTime? tuNgay, DateTime? denNgay)
        {
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            DateTime startDate = tuNgay ?? new DateTime(endDate.Year, endDate.Month, 1);

            var data = await _context.DonXinNghis
                .Include(d => d.NhanSu).ThenInclude(n => n.PhongBan)
                .Where(d => d.TrangThai == "Đã duyệt" && d.TuNgay.Date >= startDate && d.TuNgay.Date <= endDate)
                .GroupBy(d => d.NhanSu.PhongBan.TenPhongBan)
                .Select(g => new { PhongBan = g.Key ?? "Chưa cập nhật", SoNgay = g.Sum(x => x.SoNgayNghi) })
                .ToListAsync();

            return Json(new { success = true, labels = data.Select(x => x.PhongBan), data = data.Select(x => x.SoNgay) });
        }

        // ==========================================
        // 4. API BIỂU ĐỒ TRÒN: PHÂN TÍCH LOẠI NGHỈ
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetApiPhanTichLoaiNghi(DateTime? tuNgay, DateTime? denNgay)
        {
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            DateTime startDate = tuNgay ?? new DateTime(endDate.Year, endDate.Month, 1);

            var data = await _context.DonXinNghis
                .Include(d => d.LoaiNghiPhep)
                .Where(d => d.TrangThai == "Đã duyệt" && d.TuNgay.Date >= startDate && d.TuNgay.Date <= endDate)
                .GroupBy(d => d.LoaiNghiPhep.TenLoai)
                .Select(g => new { LoaiNghi = g.Key ?? "Khác", SoDon = g.Count() })
                .ToListAsync();

            return Json(new { success = true, labels = data.Select(x => x.LoaiNghi), data = data.Select(x => x.SoDon) });
        }

        // ==========================================
        // 5. API BIỂU ĐỒ CỘT: LÀM THÊM THEO PHÒNG BAN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetApiLamThemTheoPhongBan(DateTime? tuNgay, DateTime? denNgay)
        {
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            DateTime startDate = tuNgay ?? new DateTime(endDate.Year, endDate.Month, 1);

            var data = await GetBaseAttendanceQuery(startDate, endDate, "")
                .Where(c => c.GioOT > 0)
                .GroupBy(c => c.BangChamCongNhanVien.NhanSu.PhongBan.TenPhongBan)
                .Select(g => new { PhongBan = g.Key ?? "Chưa cập nhật", TongGio = Math.Round(g.Sum(x => x.GioOT), 2) })
                .ToListAsync();

            return Json(new { success = true, labels = data.Select(x => x.PhongBan), data = data.Select(x => x.TongGio) });
        }

        // ==========================================
        // 6. API BIỂU ĐỒ CỘT: ĐI MUỘN THEO PHÒNG BAN
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetApiDiMuonTheoPhongBan(DateTime? tuNgay, DateTime? denNgay)
        {
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            DateTime startDate = tuNgay ?? new DateTime(endDate.Year, endDate.Month, 1);

            var data = await GetBaseAttendanceQuery(startDate, endDate, "")
                .Where(c => c.PhutDiMuon > 0 || c.PhutVeSom > 0)
                .GroupBy(c => c.BangChamCongNhanVien.NhanSu.PhongBan.TenPhongBan)
                .Select(g => new { PhongBan = g.Key ?? "Chưa cập nhật", SoLan = g.Count() })
                .ToListAsync();

            return Json(new { success = true, labels = data.Select(x => x.PhongBan), data = data.Select(x => x.SoLan) });
        }
    }
}