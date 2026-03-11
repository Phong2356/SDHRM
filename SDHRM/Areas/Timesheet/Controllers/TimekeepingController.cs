using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDHRM.Areas.Timesheet.Controllers
{
    [Area("Timesheet")]
    [Authorize(Policy = "Timesheet.View")]
    public class TimekeepingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimekeepingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> TimesheetDetail()
        {
            var danhSach = await _context.BangChamCongs
                .OrderByDescending(b => b.TuNgay)
                .ToListAsync();

            return View(danhSach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> Create(BangChamCong model)
        {
            ModelState.Remove("TrangThai");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập!";
                return RedirectToAction(nameof(TimesheetDetail));
            }

            try
            {
                model.TrangThai = "Chưa khóa";
                model.NgayTao = DateTime.Now;

                // Lưu Bảng cấp Cha
                _context.BangChamCongs.Add(model);
                await _context.SaveChangesAsync();

                // Lấy toàn bộ nhân sự đang làm việc (Bạn có thể thêm điều kiện .Where(n => n.TrangThai == "Đang làm việc") nếu có)
                var tatCaNhanSu = await _context.NhanSus.ToListAsync();

                foreach (var nv in tatCaNhanSu)
                {
                    // Tạo Bảng cấp Con (Tổng hợp tháng)
                    var bangNV = new BangChamCongNhanVien
                    {
                        BangChamCongId = model.Id,
                        NhanSuId = nv.Id,
                        ChiTietNgays = new List<ChiTietChamCongNgay>()
                    };

                    // Chạy vòng lặp đẻ Bảng cấp Cháu (Từ TuNgay đến DenNgay)
                    for (var day = model.TuNgay.Date; day <= model.DenNgay.Date; day = day.AddDays(1))
                    {
                        bangNV.ChiTietNgays.Add(new ChiTietChamCongNgay
                        {
                            Ngay = day,
                            KyHieuChamCong = "" // Mặc định rỗng, chờ bấm nút Cập nhật (SyncData)
                        });
                    }

                    _context.BangChamCongNhanViens.Add(bangNV);
                }

                // Lưu một lần duy nhất xuống Database để tối ưu hiệu năng
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Khởi tạo bảng chấm công thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction(nameof(TimesheetDetail));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var bangChamCong = await _context.BangChamCongs.FirstOrDefaultAsync(b => b.Id == id);
            if (bangChamCong == null) return NotFound();

            // Load toàn bộ dữ liệu Cấp Con (Nhân sự) và Cấp Cháu (Chi tiết 31 ngày)
            var danhSachChamCong = await _context.BangChamCongNhanViens
                .Include(b => b.NhanSu)
                .Include(b => b.ChiTietNgays.OrderBy(c => c.Ngay)) // Bắt buộc OrderBy ngày
                .Where(b => b.BangChamCongId == id)
                .ToListAsync();

            // Sinh list cột ngày tháng để in lên thead của Table HTML
            var cacNgayTrongThang = new List<DateTime>();
            for (var d = bangChamCong.TuNgay.Date; d <= bangChamCong.DenNgay.Date; d = d.AddDays(1))
            {
                cacNgayTrongThang.Add(d);
            }

            ViewBag.BangChamCong = bangChamCong;
            ViewBag.CacNgay = cacNgayTrongThang;

            return View(danhSachChamCong);
        }

        [HttpPost]
        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> SyncData(int bangChamCongId)
        {
            var bangChamCong = await _context.BangChamCongs.FindAsync(bangChamCongId);
            if (bangChamCong == null || bangChamCong.TrangThai == "Đã khóa")
            {
                TempData["ErrorMessage"] = "Bảng chấm công không tồn tại hoặc đã bị khóa!";
                return RedirectToAction(nameof(Detail), new { id = bangChamCongId });
            }

            var tuNgay = bangChamCong.TuNgay.Date;
            var denNgay = bangChamCong.DenNgay.Date;

            var lichSuCuaThang = await _context.LichSuChamCongs
                .Where(l => l.ThoiGianCham.Date >= tuNgay && l.ThoiGianCham.Date <= denNgay)
                .ToListAsync();

            var donXinNghiCuaThang = await _context.DonXinNghis
                .Where(d => d.TrangThai == "Đã duyệt" && d.TuNgay.Date <= denNgay && d.DenNgay.Date >= tuNgay)
                .ToListAsync();

            // (Lấy luôn Đơn tăng ca nếu có)
            var donTangCaCuaThang = await _context.DonTangCas
                .Where(d => d.TrangThai == "Đã duyệt" && d.NgayTangCa.Date >= tuNgay && d.NgayTangCa.Date <= denNgay)
                .ToListAsync();

            // Lấy toàn bộ lưới chi tiết đang rỗng trên giao diện
            var tatCaChiTiet = await _context.ChiTietChamCongNgays
                .Include(c => c.BangChamCongNhanVien)
                .Where(c => c.BangChamCongNhanVien.BangChamCongId == bangChamCongId)
                .ToListAsync();

            foreach (var chiTiet in tatCaChiTiet)
            {
                int nhanSuId = chiTiet.BangChamCongNhanVien.NhanSuId;
                DateTime ngay = chiTiet.Ngay.Date;

                // Reset lại dữ liệu (Phòng trường hợp bấm Cập nhật nhiều lần)
                chiTiet.GioVao = null; chiTiet.GioRa = null;
                chiTiet.PhutDiMuon = 0; chiTiet.PhutVeSom = 0; chiTiet.GioOT = 0;
                chiTiet.SoCongGhiNhan = 0; chiTiet.KyHieuChamCong = "";

                // --- BƯỚC 1: Xử lý Ngày nghỉ (Chủ nhật) ---
                if (ngay.DayOfWeek == DayOfWeek.Sunday)
                {
                    chiTiet.KyHieuChamCong = "NT"; // Nghỉ tuần
                    continue;
                }

                // --- BƯỚC 2: Kiểm tra Đơn Xin Nghỉ ---
                var donNghi = donXinNghiCuaThang.FirstOrDefault(d => d.NhanSuId == nhanSuId && d.TuNgay.Date <= ngay && d.DenNgay.Date >= ngay);

                if (donNghi != null)
                {
                    if (donNghi.LoaiNghiPhepId == 2)
                    {
                        chiTiet.KyHieuChamCong = "KL";
                        chiTiet.SoCongGhiNhan = 0;     
                    }
                    else
                    {
                        chiTiet.KyHieuChamCong = "P";  
                        chiTiet.SoCongGhiNhan = 1;    
                    }
                    continue;
                }

                // --- BƯỚC 3: Móc dữ liệu Máy vân tay (Bảng LichSuChamCong) ---
                var lichSuTrongNgay = lichSuCuaThang.Where(l => l.NhanSuId == nhanSuId && l.ThoiGianCham.Date == ngay).ToList();

                if (lichSuTrongNgay.Any())
                {
                    chiTiet.GioVao = lichSuTrongNgay.Min(l => l.ThoiGianCham).TimeOfDay;
                    chiTiet.GioRa = lichSuTrongNgay.Max(l => l.ThoiGianCham).TimeOfDay;

                    var gioHanhChinhBatDau = new TimeSpan(8, 0, 0);
                    var gioHanhChinhKetThuc = new TimeSpan(17, 0, 0);

                    if (chiTiet.GioVao == chiTiet.GioRa)
                    {
                        chiTiet.KyHieuChamCong = "K"; // Khuyết (Quên bấm giờ ra)
                        chiTiet.SoCongGhiNhan = 0.5;
                    }
                    else if (chiTiet.GioVao <= gioHanhChinhBatDau.Add(TimeSpan.FromMinutes(5)) && chiTiet.GioRa >= gioHanhChinhKetThuc)
                    {
                        chiTiet.KyHieuChamCong = "X"; // Đủ công
                        chiTiet.SoCongGhiNhan = 1;
                    }
                    else
                    {
                        chiTiet.KyHieuChamCong = "MV"; // Đi muộn / Về sớm
                        chiTiet.SoCongGhiNhan = 1;

                        if (chiTiet.GioVao > gioHanhChinhBatDau)
                            chiTiet.PhutDiMuon = (int)(chiTiet.GioVao.Value - gioHanhChinhBatDau).TotalMinutes;

                        if (chiTiet.GioRa < gioHanhChinhKetThuc)
                            chiTiet.PhutVeSom = (int)(gioHanhChinhKetThuc - chiTiet.GioRa.Value).TotalMinutes;
                    }
                }
                else
                {
                    chiTiet.KyHieuChamCong = "V"; // Vắng không lý do
                    chiTiet.SoCongGhiNhan = 0;
                }

                // --- BƯỚC Phụ: Cập nhật Giờ Làm Thêm (OT) ---
                var donOT = donTangCaCuaThang.FirstOrDefault(d => d.NhanSuId == nhanSuId && d.NgayTangCa.Date == ngay);
                if (donOT != null)
                {
                    chiTiet.GioOT = donOT.SoGio;
                }
            }

            // ----------------------------------------------------------------------
            // BƯỚC 4: TÍNH TỔNG ĐẨY NGƯỢC LÊN BẢNG CẤP CON (Để tính lương)
            // ----------------------------------------------------------------------
            var danhSachBangNV = await _context.BangChamCongNhanViens
                .Include(b => b.ChiTietNgays)
                .Where(b => b.BangChamCongId == bangChamCongId)
                .ToListAsync();

            foreach (var bangNV in danhSachBangNV)
            {
                // Các cột cũ giữ nguyên
                bangNV.TongCongThucTe = bangNV.ChiTietNgays.Where(c => c.KyHieuChamCong == "X" || c.KyHieuChamCong == "MV" || c.KyHieuChamCong == "K").Sum(c => c.SoCongGhiNhan);
                bangNV.TongCongNghiPhep = bangNV.ChiTietNgays.Where(c => c.KyHieuChamCong == "P").Sum(c => c.SoCongGhiNhan);

                // THÊM MỚI: Đếm số ngày Nghỉ không lương (Ký hiệu KL)
                bangNV.TongCongKhongLuong = bangNV.ChiTietNgays.Count(c => c.KyHieuChamCong == "KL");

                bangNV.TongPhutDiMuonVeSom = bangNV.ChiTietNgays.Sum(c => c.PhutDiMuon + c.PhutVeSom);
                bangNV.TongGioTangCa = bangNV.ChiTietNgays.Sum(c => c.GioOT);

                // Công hưởng lương lúc này đã hoàn toàn chuẩn xác (Chỉ gồm Thực tế + Phép có lương)
                bangNV.TongCongHuongLuong = bangNV.TongCongThucTe + bangNV.TongCongNghiPhep + bangNV.TongCongLeTet;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đồng bộ dữ liệu chấm công thành công!";
            return RedirectToAction(nameof(Detail), new { id = bangChamCongId });
        }

        [HttpPost]
        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var bangChamCong = await _context.BangChamCongs.FindAsync(id);
            if (bangChamCong == null) return NotFound();

            if (bangChamCong.TrangThai == "Đã khóa")
            {
                TempData["ErrorMessage"] = "Không thể xóa bảng chấm công đã khóa!";
                return RedirectToAction(nameof(TimesheetDetail));
            }

            // Xóa Bảng Cha (Entity Framework sẽ tự động xóa kiểu Cascade xóa luôn Con và Cháu)
            _context.BangChamCongs.Remove(bangChamCong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa bảng chấm công!";
            return RedirectToAction(nameof(TimesheetDetail));
        }


        public async Task<IActionResult> Timekeeper(DateTime? ngayChamCong)
        {
            DateTime selectedDate = ngayChamCong ?? DateTime.Now.Date;

            var lichSu = await _context.LichSuChamCongs
                .Include(l => l.NhanSu)
                .ThenInclude(n => n.PhongBan)
                .Where(l => l.ThoiGianCham.Date == selectedDate.Date)
                .OrderByDescending(l => l.ThoiGianCham) 
                .ToListAsync();

            ViewBag.NgayChamCong = selectedDate;

            return View(lichSu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> ChotBangChamCong(int bangChamCongId)
        {
            var bangChamCong = await _context.BangChamCongs.FindAsync(bangChamCongId);
            if (bangChamCong == null) return NotFound();

            // THÊM ĐOẠN KIỂM TRA NÀY VÀO:
            if (bangChamCong.TrangThai == "Đã khóa")
            {
                TempData["ErrorMessage"] = "Lỗi! Bảng chấm công này đã được chốt và khóa từ trước!";
                return RedirectToAction(nameof(Detail), new { id = bangChamCongId });
            }

            // Cập nhật trạng thái thành Đã khóa
            bangChamCong.TrangThai = "Đã khóa";
            _context.Update(bangChamCong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã chốt bảng chấm công! Bạn có thể chuyển sang phân hệ Tiền lương để tiếp tục.";

            return RedirectToAction(nameof(Detail), new { id = bangChamCongId });
        }
    }
}