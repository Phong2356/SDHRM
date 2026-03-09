using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using SDHRM.Models.ViewModels;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class TimesheetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TimesheetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. HÀM LOAD GIAO DIỆN
        public async Task<IActionResult> Timekeeping()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null) return Challenge();

            var nhansu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);
            if (nhansu == null) return NotFound("Chưa có hồ sơ.");

            // Tìm Thứ 2 đầu tuần này
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7); // Hết Chủ nhật

            // Truy vấn lịch sử chấm công CỦA TUẦN NÀY để nhét vào View
            var history = await _context.LichSuChamCongs
                .Where(x => x.NhanSuId == nhansu.Id && x.ThoiGianCham >= startOfWeek && x.ThoiGianCham < endOfWeek)
                .ToListAsync();

            return View(history);
        }

        // 2. HÀM XỬ LÝ KHI BẤM NÚT "CHẤM CÔNG"
        [HttpPost]
        public async Task<IActionResult> XuLyChamCong(string loai, string ipPublic, string hinhAnh)
        {
            try
            {
                // 1. CHUẨN HÓA DỮ LIỆU TỪ JAVASCRIPT
                // Nếu Frontend gửi lên "vao" thì gán là "CheckIn", gửi "ra" thì gán "CheckOut"
                string loaiChuan = (loai == "vao") ? "CheckIn" : "CheckOut";

                var currentUserId = _userManager.GetUserId(User);
                if (currentUserId == null) return Json(new { success = false, message = "Vui lòng đăng nhập!" });

                var nhansu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);
                if (nhansu == null) return Json(new { success = false, message = "Lỗi: Tài khoản chưa có hồ sơ nhân sự!" });

                var hopLe = await _context.CauHinhWifis.AnyAsync(w => w.DiaChiIP == ipPublic && w.TrangThai == true);
                if (!hopLe) return Json(new { success = false, message = $"Sai WiFi công ty. (IP của bạn: {ipPublic})" });

                // =========================================================
                // 2. XỬ LÝ LƯU ẢNH TỪ CAMERA GỬI LÊN
                // =========================================================
                string duongDanAnh = null;
                if (!string.IsNullOrEmpty(hinhAnh))
                {
                    try
                    {
                        var base64Data = hinhAnh.Contains(",") ? hinhAnh.Split(',')[1] : hinhAnh;
                        var bytes = Convert.FromBase64String(base64Data);

                        var fileName = $"ChamCong_{nhansu.Id}_{DateTime.Now:yyyyMMddHHmmss}.jpg";

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chamcong");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(uploadsFolder, fileName);
                        await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                        duongDanAnh = $"/uploads/chamcong/{fileName}";
                    }
                    catch { }
                }

                // =========================================================
                // 3. LOGIC CHẤM CÔNG (Sử dụng biến loaiChuan)
                // =========================================================
                DateTime today = DateTime.Today;

                // Sửa loai => loaiChuan ở đây
                var logCu = await _context.LichSuChamCongs.FirstOrDefaultAsync(x =>
                    x.NhanSuId == nhansu.Id &&
                    x.LoaiChamCong == loaiChuan &&
                    x.ThoiGianCham.Date == today);

                if (logCu != null)
                {
                    if (loaiChuan == "CheckIn")
                    {
                        return Json(new { success = false, message = $"Bạn đã chấm công ĐẾN lúc {logCu.ThoiGianCham:HH:mm}!" });
                    }
                    else // CheckOut: Cập nhật đè lên giờ về cũ
                    {
                        logCu.ThoiGianCham = DateTime.Now;
                        logCu.IPNguoiDung = ipPublic;
                        if (duongDanAnh != null) logCu.AnhChamCong = duongDanAnh; // Cập nhật ảnh về mới nhất

                        _context.LichSuChamCongs.Update(logCu);
                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = $"Đã cập nhật giờ VỀ mới nhất lúc {DateTime.Now:HH:mm}!" });
                    }
                }
                else // Chưa có log nào trong ngày
                {
                    var log = new LichSuChamCong
                    {
                        NhanSuId = nhansu.Id,
                        ThoiGianCham = DateTime.Now,
                        LoaiChamCong = loaiChuan, // Lưu đúng chuẩn CheckIn/CheckOut vào DB
                        IPNguoiDung = ipPublic,
                        TrangThai = "Hợp lệ",
                        AnhChamCong = duongDanAnh
                    };

                    _context.LichSuChamCongs.Add(log);
                    await _context.SaveChangesAsync();

                    string tenLoai = loaiChuan == "CheckIn" ? "ĐẾN" : "VỀ";
                    return Json(new { success = true, message = $"Chấm công {tenLoai} thành công!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi Server: " + ex.Message);
            }
        }

        // THÊM THAM SỐ month VÀ year ĐỂ ĐIỀU HƯỚNG THÁNG
        public async Task<IActionResult> TimesheetGrid(int? month, int? year)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null) return Challenge();

            var nhansu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);
            if (nhansu == null) return NotFound("Chưa có hồ sơ nhân sự.");

            // 1. Xác định tháng/năm cần xem (Mặc định là tháng hiện tại)
            DateTime today = DateTime.Today;
            int targetMonth = month ?? today.Month;
            int targetYear = year ?? today.Year;

            // 2. Tính ngày đầu tiên của tháng
            DateTime firstDayOfMonth = new DateTime(targetYear, targetMonth, 1);

            // 3. Tính ngày bắt đầu vẽ LƯỚI (Lùi về Thứ 2 gần nhất)
            int deltaBefore = (int)firstDayOfMonth.DayOfWeek;
            if (deltaBefore == 0) deltaBefore = 7; // Chủ nhật trong C# là 0, ta đổi thành 7
            DateTime startGridDate = firstDayOfMonth.AddDays(-(deltaBefore - 1));

            // Lưới lịch chuẩn luôn có 42 ô (6 tuần x 7 ngày)
            DateTime endGridDate = startGridDate.AddDays(41);

            // 4. Lấy lịch sử chấm công trong khoảng thời gian của lưới lịch
            var logs = await _context.LichSuChamCongs
                .Where(x => x.NhanSuId == nhansu.Id
                         && x.ThoiGianCham.Date >= startGridDate
                         && x.ThoiGianCham.Date <= endGridDate)
                .ToListAsync();

            // 5. Đổ dữ liệu vào ViewModel
            var model = new MonthlyAttendanceViewModel
            {
                Month = targetMonth,
                Year = targetYear,
                Days = new List<CalendarDay>()
            };

            for (var date = startGridDate; date <= endGridDate; date = date.AddDays(1))
            {
                // Tìm các lượt chấm công của ngày hiện tại trong vòng lặp
                var logsInDay = logs.Where(x => x.ThoiGianCham.Date == date).ToList();

                var dayInfo = new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = date.Month == targetMonth,
                    IsToday = date == today,
                    // Lấy giờ ĐẾN sớm nhất
                    GioVao = logsInDay.Where(x => x.LoaiChamCong == "CheckIn")
                                      .OrderBy(x => x.ThoiGianCham)
                                      .FirstOrDefault()?.ThoiGianCham.TimeOfDay,
                    // Lấy giờ VỀ muộn nhất
                    GioRa = logsInDay.Where(x => x.LoaiChamCong == "CheckOut")
                                     .OrderByDescending(x => x.ThoiGianCham)
                                     .FirstOrDefault()?.ThoiGianCham.TimeOfDay
                };
                model.Days.Add(dayInfo);
            }

            return View(model);
        }
    }
}
