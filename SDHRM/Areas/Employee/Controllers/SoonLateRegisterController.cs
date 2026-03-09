using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SDHRM.Models;
using SDHRM.Data;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class SoonLateRegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SoonLateRegisterController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Xem lịch sử đơn của bản thân
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            var danhSachDon = await _context.DonDiMuonVeSoms
                .Include(d => d.NguoiDuyet)
                .Where(d => d.NhanSuId == nhanSu.Id)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return View(danhSachDon);
        }

        // 2. Mở form làm đơn mới
        [HttpGet]
        public IActionResult Create()
        {
            var model = new DonDiMuonVeSom
            {
                NgayApDung = DateTime.Today,
                SoPhut = 0,
                LoaiDangKy = "Đi muộn",
                LyDo = string.Empty
            };
            return View(model);
        }

        // 3. Xử lý lưu đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonDiMuonVeSom model)
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            // Gỡ validate các trường ngầm định
            ModelState.Remove("NhanSu");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                model.NhanSuId = nhanSu.Id;
                model.TrangThai = "Chờ duyệt";
                model.NgayTao = DateTime.Now;

                _context.DonDiMuonVeSoms.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã gửi đơn xin đi muộn/về sớm thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            // Tìm đơn đúng của nhân viên đang đăng nhập
            var don = await _context.DonDiMuonVeSoms
                .FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);

            if (don == null) return NotFound("Không tìm thấy đơn hoặc bạn không có quyền sửa!");

            // LUẬT: Chỉ cho sửa khi đang "Chờ duyệt"
            if (don.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] = "Đơn đã được xử lý, bạn không thể chỉnh sửa!";
                return RedirectToAction(nameof(Index));
            }

            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonDiMuonVeSom model)
        {
            if (id != model.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            // Lấy đơn gốc từ DB ra để kiểm tra bảo mật
            var donGoc = await _context.DonDiMuonVeSoms.FindAsync(id);
            if (donGoc == null || donGoc.NhanSuId != nhanSu.Id) return NotFound();

            if (donGoc.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] = "Lỗi bảo mật: Đơn đã xử lý không thể lưu thay đổi!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("NhanSu");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                // Chỉ cập nhật những trường được phép
                donGoc.LoaiDangKy = model.LoaiDangKy;
                donGoc.NgayApDung = model.NgayApDung;
                donGoc.SoPhut = model.SoPhut;
                donGoc.LyDo = model.LyDo;
                // Nếu sửa thì mặc định vẫn là chờ duyệt, không cần gán lại

                _context.Update(donGoc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật đơn thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

                var don = await _context.DonDiMuonVeSoms
                    .FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);

                if (don == null) return Json(new { success = false, message = "Không tìm thấy đơn!" });

                if (don.TrangThai != "Chờ duyệt")
                    return Json(new { success = false, message = "Chỉ có thể xóa đơn đang ở trạng thái Chờ duyệt!" });

                _context.DonDiMuonVeSoms.Remove(don);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa đơn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    } 
}
