using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SDHRM.Models;
using SDHRM.Data;   

namespace SDHRM.Areas.Timesheet.Controllers
{
    [Area("Timesheet")]
    public class ManagementRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagementRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> Attendance()
        {
            var danhSachDon = await _context.DonXinNghis
                .Include(d => d.NhanSu)
                .Include(d => d.LoaiNghiPhep)
                .Include(d => d.NguoiDuyet)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return View(danhSachDon);
        }


        [HttpPost]
        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> DuyetDonXinNghi(int donId, string trangThai, string ghiChu)
        {
            // 1. Tìm đơn
            var don = await _context.DonXinNghis
                .Include(d => d.LoaiNghiPhep)
                .FirstOrDefaultAsync(d => d.Id == donId);

            if (don == null) return Json(new { success = false, message = "Không tìm thấy đơn!" });

            // Tránh việc duyệt đúp 2 lần
            if (don.TrangThai != "Chờ duyệt")
                return Json(new { success = false, message = "Đơn này đã được xử lý rồi!" });

            // 2. Xác định Quản lý/HR đang đăng nhập
            var currentUserId = _userManager.GetUserId(User);
            var nguoiDuyet = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);

            if (nguoiDuyet == null) return Json(new { success = false, message = "Không xác định được danh tính người duyệt!" });

            // 3. Cập nhật thông tin đơn
            don.TrangThai = trangThai; // Sẽ nhận "Đã duyệt" hoặc "Từ chối" từ View gửi lên
            don.NguoiDuyetId = nguoiDuyet.Id;
            don.GhiChuCuaNguoiDuyet = ghiChu;

            // 4. KIẾM TRA QUỸ PHÉP NẾU DUYỆT
            if (trangThai == "Đã duyệt" && don.LoaiNghiPhep != null && don.LoaiNghiPhep.TruVaoQuyPhep)
            {
                var quyPhep = await _context.QuyPhepNhanViens
                    .FirstOrDefaultAsync(q => q.NhanSuId == don.NhanSuId && q.Nam == don.TuNgay.Year);

                if (quyPhep != null)
                {
                    // Tăng số phép đã sử dụng lên
                    quyPhep.SoPhepDaDung += don.SoNgayNghi;
                    _context.Update(quyPhep);
                }
            }

            _context.Update(don);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xử lý đơn thành công!" });
        }

        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> LeaveSummary(int? year)
        {
            // Mặc định lấy năm hiện tại nếu không chọn
            int selectedYear = year ?? DateTime.Now.Year;
            ViewBag.SelectedYear = selectedYear;

            // Lấy danh sách quỹ phép kèm thông tin nhân sự
            var danhSachQuyPhep = await _context.QuyPhepNhanViens
             .Include(q => q.NhanSu)
                 .ThenInclude(n => n.ViTriCongViec)
             .Include(q => q.NhanSu)
                 .ThenInclude(n => n.PhongBan)
             .Where(q => q.Nam == selectedYear)
             .ToListAsync();

            return View(danhSachQuyPhep);
        }

        [HttpPost]
        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> UpdateBalance(int id, double tongPhep, double phepTon, double phepThamNien, double phepThuong, double daDung)
        {
            try
            {
                var quyPhep = await _context.QuyPhepNhanViens.FindAsync(id);
                if (quyPhep == null)
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu quỹ phép này!" });

                // Công thức tính tổng phép có được
                double tongCong = tongPhep + phepTon + phepThamNien + phepThuong;

                if (daDung > tongCong)
                    return Json(new { success = false, message = "Lỗi: Số phép đã dùng không thể lớn hơn Tổng số phép đang có!" });

                // Cập nhật số liệu
                quyPhep.TongPhepNam = tongPhep;
                quyPhep.PhepTonNamTruoc = phepTon;
                quyPhep.PhepThamNien = phepThamNien;
                quyPhep.PhepThuong = phepThuong;
                quyPhep.SoPhepDaDung = daDung;

                _context.Update(quyPhep);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật quỹ phép thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }


        [HttpPost]
        [Authorize(Policy = "Timesheet.Manage")]
        public async Task<IActionResult> KhoiTaoQuyPhep(int year)
        {
            var tatCaNhanSu = await _context.NhanSus.ToListAsync();
            int count = 0;
            int namTruoc = year - 1; // Nhìn lại dữ liệu năm ngoái

            foreach (var nv in tatCaNhanSu)
            {
                bool daCo = await _context.QuyPhepNhanViens.AnyAsync(q => q.NhanSuId == nv.Id && q.Nam == year);
                if (!daCo)
                {
                    // Tìm quỹ phép của năm ngoái
                    var quyPhepNamTruoc = await _context.QuyPhepNhanViens
                        .FirstOrDefaultAsync(q => q.NhanSuId == nv.Id && q.Nam == namTruoc);

                    double soPhepKetChuyen = 0;

                    if (quyPhepNamTruoc != null && quyPhepNamTruoc.SoPhepConLai > 0)
                    {
                        // LUẬT: Tối đa chỉ được chuyển 5 ngày sang năm mới
                        soPhepKetChuyen = quyPhepNamTruoc.SoPhepConLai > 5 ? 5 : quyPhepNamTruoc.SoPhepConLai;
                    }

                    _context.QuyPhepNhanViens.Add(new QuyPhepNhanVien
                    {
                        NhanSuId = nv.Id,
                        Nam = year,
                        TongPhepNam = 12, // Mặc định 12 ngày
                        PhepTonNamTruoc = soPhepKetChuyen, // Tự động cộng phép dư năm cũ
                        PhepThamNien = 0,
                        PhepThuong = 0,
                        SoPhepDaDung = 0
                    });
                    count++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã tạo quỹ phép năm {year} và kết chuyển phép năm {namTruoc} cho {count} nhân viên!";

            // Redirect về action LeaveSummary mới
            return RedirectToAction(nameof(LeaveSummary), new { year = year });
        }

        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> LateInEarlyOut()
        {
            // Lấy danh sách đơn, join với bảng NhanSu (người tạo) và NguoiDuyet (người duyệt nếu có)
            var danhSachDon = await _context.DonDiMuonVeSoms
                .Include(d => d.NhanSu)
                .Include(d => d.NguoiDuyet)
                .OrderByDescending(d => d.NgayTao) // Đơn mới nhất xếp lên đầu
                .ToListAsync();

            return View(danhSachDon);
        }

        [HttpPost]
        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> XuLyDon(int id, string trangThai, string ghiChu)
        {
            // 1. Lấy mã ID của tài khoản đang đăng nhập từ Identity
            var currentUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để duyệt đơn!";
                return RedirectToAction(nameof(LateInEarlyOut));
            }

            // 2. Đối chiếu ID tài khoản với bảng Nhân sự
            var nguoiDuyet = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);

            if (nguoiDuyet == null)
            {
                // DÒNG BÁO LỖI NÀY SẼ CHỈ ĐIỂM CHÍNH XÁC VẤN ĐỀ CỦA BẠN
                TempData["ErrorMessage"] = $"Lỗi: Tài khoản Identity của bạn (ID: {currentUserId}) chưa được liên kết với bất kỳ hồ sơ Nhân sự nào!";
                return RedirectToAction(nameof(LateInEarlyOut));
            }

            // 3. Tiến hành lưu dữ liệu
            var don = await _context.DonDiMuonVeSoms.FindAsync(id);

            if (don != null)
            {
                don.TrangThai = trangThai;
                don.GhiChuCuaNguoiDuyet = ghiChu;
                don.NguoiDuyetId = nguoiDuyet.Id;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã {trangThai.ToLower()} đơn thành công!";
            }

            // Quay lại màn hình danh sách
            return RedirectToAction(nameof(LateInEarlyOut));
        }

        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> Overtime()
        {
            // Lấy danh sách đơn, join với bảng NhanSu và lấy luôn PhongBan
            var danhSachDon = await _context.DonTangCas
                .Include(d => d.NhanSu)
                    .ThenInclude(n => n.PhongBan)
                .Include(d => d.NguoiDuyet)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return View(danhSachDon);
        }

        [HttpPost]
        public async Task<IActionResult> XuLyDonTangCa(int id, string trangThai, string ghiChu)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để duyệt đơn!";
                return RedirectToAction(nameof(Overtime));
            }

            var nguoiDuyet = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);
            if (nguoiDuyet == null)
            {
                TempData["ErrorMessage"] = $"Lỗi: Tài khoản Identity (ID: {currentUserId}) chưa liên kết hồ sơ Nhân sự!";
                return RedirectToAction(nameof(Overtime));
            }

            var don = await _context.DonTangCas.FindAsync(id);
            if (don != null)
            {
                don.TrangThai = trangThai;
                don.GhiChuCuaNguoiDuyet = ghiChu;
                don.NguoiDuyetId = nguoiDuyet.Id;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã {trangThai.ToLower()} đơn tăng ca thành công!";
            }

            return RedirectToAction(nameof(Overtime));
        }

        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> UpdateWorking()
        {
            // Lấy danh sách, join bảng Nhân sự và Phòng ban
            var danhSachDon = await _context.DeNghiCapNhatCongs
                .Include(d => d.NhanSu)
                    .ThenInclude(n => n.PhongBan)
                .Include(d => d.NguoiDuyet)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return View(danhSachDon);
        }

        [HttpPost]
        [Authorize(Policy = "Timesheet.Approve")]
        public async Task<IActionResult> XuLyDonCapNhatCong(int id, string trangThai, string ghiChu)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để duyệt đơn!";
                return RedirectToAction(nameof(UpdateWorking));
            }

            var nguoiDuyet = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);
            if (nguoiDuyet == null)
            {
                TempData["ErrorMessage"] = $"Lỗi: Tài khoản Identity (ID: {currentUserId}) chưa liên kết hồ sơ Nhân sự!";
                return RedirectToAction(nameof(UpdateWorking));
            }

            var don = await _context.DeNghiCapNhatCongs.FindAsync(id);
            if (don != null)
            {
                don.TrangThai = trangThai;
                don.GhiChuCuaNguoiDuyet = ghiChu;
                don.NguoiDuyetId = nguoiDuyet.Id;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã {trangThai.ToLower()} đề nghị cập nhật công thành công!";
            }

            return RedirectToAction(nameof(UpdateWorking));
        }
    }
}