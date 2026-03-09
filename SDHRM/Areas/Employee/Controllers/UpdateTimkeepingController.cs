using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SDHRM.Models;
using SDHRM.Data;
using Microsoft.AspNetCore.Hosting;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class UpdateTimkeepingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public UpdateTimkeepingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // 1. Xem lịch sử các đơn cập nhật công
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            var danhSachDon = await _context.DeNghiCapNhatCongs
                .Include(d => d.NguoiDuyet)
                .Where(d => d.NhanSuId == nhanSu.Id)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return View(danhSachDon);
        }

        // 2. Mở form làm đơn
        [HttpGet]
        public IActionResult Create()
        {
            var model = new DeNghiCapNhatCong
            {
                NgayCapNhat = DateTime.Today,
                LyDo = string.Empty
            };
            return View(model);
        }

        // 3. Xử lý lưu đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeNghiCapNhatCong model, IFormFile? fileUpload)
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            // Gỡ bỏ kiểm tra các trường tự động
            ModelState.Remove("NhanSu");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");
            ModelState.Remove("FileDinhKem");
            ModelState.Remove("fileUpload");

            if (ModelState.IsValid)
            {
                // Kiểm tra logic: Phải nhập ít nhất Giờ vào HOẶC Giờ ra
                if (model.GioVao == null && model.GioRa == null)
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập ít nhất Giờ vào hoặc Giờ ra để cập nhật!");
                    return View(model);
                }

                model.NhanSuId = nhanSu.Id;
                model.TrangThai = "Chờ duyệt";
                model.NgayTao = DateTime.Now;

                // Xử lý upload file minh chứng (VD: Ảnh chụp màn hình tin nhắn xin phép sếp)
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "capnhatcong");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }
                    model.FileDinhKem = "/uploads/capnhatcong/" + uniqueFileName;
                }

                _context.DeNghiCapNhatCongs.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã gửi đề nghị cập nhật công thành công!";
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
            var don = await _context.DeNghiCapNhatCongs
                .FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);

            if (don == null) return NotFound("Không tìm thấy đơn hoặc bạn không có quyền sửa!");

            // LUẬT BẢO MẬT: Chỉ cho sửa khi đang "Chờ duyệt"
            if (don.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] = "Đơn đã được xử lý, bạn không thể chỉnh sửa!";
                return RedirectToAction(nameof(Index));
            }

            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeNghiCapNhatCong model, IFormFile? fileUpload)
        {
            if (id != model.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            // Lấy đơn gốc từ DB ra để đối chiếu
            var donGoc = await _context.DeNghiCapNhatCongs.FindAsync(id);
            if (donGoc == null || donGoc.NhanSuId != nhanSu.Id) return NotFound();

            if (donGoc.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] = "Lỗi bảo mật: Đơn đã xử lý không thể lưu thay đổi!";
                return RedirectToAction(nameof(Index));
            }

            // Gỡ validate các trường không cần thiết trên form
            ModelState.Remove("NhanSu");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");
            ModelState.Remove("FileDinhKem");
            ModelState.Remove("fileUpload");

            if (ModelState.IsValid)
            {
                if (model.GioVao == null && model.GioRa == null)
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập ít nhất Giờ vào hoặc Giờ ra để cập nhật!");
                    return View(model);
                }

                // Cập nhật thông tin
                donGoc.NgayCapNhat = model.NgayCapNhat;
                donGoc.GioVao = model.GioVao;
                donGoc.GioRa = model.GioRa;
                donGoc.LyDo = model.LyDo;

                // Xử lý nếu người dùng upload file minh chứng MỚI
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "capnhatcong");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }
                    // Ghi đè đường dẫn file mới
                    donGoc.FileDinhKem = "/uploads/capnhatcong/" + uniqueFileName;
                }

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

                var don = await _context.DeNghiCapNhatCongs
                    .FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);

                if (don == null) return Json(new { success = false, message = "Không tìm thấy đơn!" });

                if (don.TrangThai != "Chờ duyệt")
                    return Json(new { success = false, message = "Chỉ có thể xóa đơn đang ở trạng thái Chờ duyệt!" });

                _context.DeNghiCapNhatCongs.Remove(don);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa đề nghị cập nhật công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
