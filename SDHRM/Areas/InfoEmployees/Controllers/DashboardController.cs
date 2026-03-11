using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDHRM.Areas.InfoEmployees.Controllers
{
    [Area("InfoEmployees")]
    [Authorize(Policy = "EmployeeInfo.View")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            // 1. Kéo dữ liệu
            var tatCaNhanSu = await _context.NhanSus.Include(n => n.PhongBan).ToListAsync();
            var tatCaHopDong = await _context.HopDongs.ToListAsync();
            var thongTinCongViec = await _context.ThongTinCongViecs.ToListAsync();

            var today = DateTime.Today;
            var currentYear = today.Year;
            var currentMonth = today.Month;
            var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var startOfLastWeek = startOfThisWeek.AddDays(-7);
            var endOfLastWeek = startOfThisWeek.AddDays(-1);

            // 2. CHỈ SỐ TỔNG QUAN
            model.TongNhanVien = thongTinCongViec.Count;
            model.NhanVienChinhThuc = thongTinCongViec.Count(t => t.TinhChatLaoDong == "Chính thức");
            model.NhanVienThuViec = thongTinCongViec.Count(t => t.TinhChatLaoDong == "Thử việc");
            model.NhanVienHocViec = thongTinCongViec.Count(t => t.TinhChatLaoDong == "Học việc");
            model.NhanVienNghiThaiSan = thongTinCongViec.Count(t => t.TrangThaiLaoDong == "Nghỉ thai sản");
            model.NhanVienKhac = model.TongNhanVien - (model.NhanVienChinhThuc + model.NhanVienThuViec + model.NhanVienHocViec + model.NhanVienNghiThaiSan);

            // 3. ĐẾM SỐ NGƯỜI CHƯA KÝ HỢP ĐỒNG
            var nhanSuDaKyHopDongIds = tatCaHopDong.Select(h => h.NhanSuId).Distinct().ToList();
            model.ChuaKyHopDong = tatCaNhanSu.Count(n => !nhanSuDaKyHopDongIds.Contains(n.Id));

            var phongBanGroup = tatCaNhanSu
                .Where(n => n.PhongBan != null && n.TrangThai != "Nghỉ việc")
                .GroupBy(n => n.PhongBan.TenPhongBan)
                .ToList();

            model.PhongBanLabels = phongBanGroup.Select(g => g.Key).ToList();
            model.PhongBanData = phongBanGroup.Select(g => g.Count()).ToList();

            int xacDinhThoiHan = tatCaHopDong.Count(h => !string.IsNullOrEmpty(h.LoaiHopDong) && h.LoaiHopDong.ToLower().Contains("xác định thời hạn"));
            int khac = tatCaHopDong.Count - xacDinhThoiHan;
            model.HopDongTheoLoai = new List<int> { xacDinhThoiHan, khac };

            // 5. CHỈ SỐ BIẾN ĐỘNG TUẦN 
            model.NhanVienMoiTuanNay = tatCaHopDong.Count(h => h.NgayCoHieuLuc >= startOfThisWeek);



            var tiepNhanMoiThang = new List<int>();
            var nghiViecMoiThang = new List<int>();
            var soLuongTongMoiThang = new List<int>();

            int luyKeNhanSu = tatCaHopDong.Count(h => h.NgayCoHieuLuc.Year < currentYear);

            for (int month = 1; month <= 12; month++)
            {
                int tiepNhanThangNay = tatCaHopDong.Count(h => h.NgayCoHieuLuc.Year == currentYear && h.NgayCoHieuLuc.Month == month);
                int nghiViecThangNay = tatCaHopDong.Count(h => h.NgayHetHan.Year == currentYear && h.NgayHetHan.Month == month);

                tiepNhanMoiThang.Add(tiepNhanThangNay);
                nghiViecMoiThang.Add(nghiViecThangNay);

                if (month > currentMonth)
                {
                    soLuongTongMoiThang.Add(0);
                }
                else
                {
                    luyKeNhanSu = luyKeNhanSu + tiepNhanThangNay - nghiViecThangNay;
                    soLuongTongMoiThang.Add(luyKeNhanSu > 0 ? luyKeNhanSu : 0);
                }
            }

            model.BienDongTiepNhan = tiepNhanMoiThang;
            model.BienDongNghiViec = nghiViecMoiThang;
            model.SoLuongNhanSuThang = soLuongTongMoiThang;

            return View(model);
        }
    }
}