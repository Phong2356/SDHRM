using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SDHRM.Data;
using SDHRM.Models;
using System;
using System.Threading.Tasks;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class OverviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OverviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            if (nhanSu == null) return View();

            ViewBag.HoTen = nhanSu.HoTen;

            // 1. Lấy đơn Đi muộn
            var donDiMuon = await _context.DonDiMuonVeSoms
                .Where(d => d.NhanSuId == nhanSu.Id && d.TrangThai == "Chờ duyệt")
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            var donXinNghi = await _context.DonXinNghis
                .Where(d => d.NhanSuId == nhanSu.Id && d.TrangThai == "Chờ duyệt")
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            var donCapNhatCong = await _context.DeNghiCapNhatCongs
                .Where(d => d.NhanSuId == nhanSu.Id && d.TrangThai == "Chờ duyệt")
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            var donTangCa = await _context.DonTangCas
                .Where(d => d.NhanSuId == nhanSu.Id && d.TrangThai == "Chờ duyệt")
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();
            // Đẩy ra ViewBag
            ViewBag.DonDiMuon = donDiMuon;
            ViewBag.DonXinNghi = donXinNghi;
            ViewBag.DonCapNhatCong = donCapNhatCong;
            ViewBag.DonTangCa = donTangCa;

            // Cập nhật lại tổng số đơn chờ
            ViewBag.TongDonCho = donDiMuon.Count + donXinNghi.Count + donCapNhatCong.Count + donTangCa.Count;
            // 3. Tính toán lại số liệu Quỹ phép (Đồng bộ với các trang khác)
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            var quyPhep = await _context.QuyPhepNhanViens
                .FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == currentYear);

            double phepCaNam = 0;
            double phepHienTai = 0;

            if (quyPhep != null)
            {
                double tongCaNam = quyPhep.TongPhepNam + quyPhep.PhepTonNamTruoc + quyPhep.PhepThamNien + quyPhep.PhepThuong;
                phepCaNam = tongCaNam - quyPhep.SoPhepDaDung;

                double phepTichLuy = Math.Round((quyPhep.TongPhepNam / 12.0) * currentMonth, 1);
                double tongDenHienTai = phepTichLuy + quyPhep.PhepTonNamTruoc + quyPhep.PhepThamNien + quyPhep.PhepThuong;
                phepHienTai = tongDenHienTai - quyPhep.SoPhepDaDung;

                if (phepHienTai < 0) phepHienTai = 0;
            }

            ViewBag.PhepConLaiCaNam = phepCaNam;
            ViewBag.PhepConLaiDenHienTai = phepHienTai;
            ViewBag.TongNgayCong = 0; // Tạm thời để 0, sau này bạn làm module Chấm công sẽ đổ data vào đây

            return View();
        }
    }
}
