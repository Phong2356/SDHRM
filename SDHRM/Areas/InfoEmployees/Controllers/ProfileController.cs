using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using NuGet.ProjectModel;
using SDHRM.Data;
using SDHRM.Models;
using SDHRM.Models.Process;
using SDHRM.Models.Services;
using SDHRM.Models.ViewModels;
using System;
using System.Threading.Tasks;

namespace SDHRM.Areas.InfoEmployees.Controllers
{
    [Area("InfoEmployees")]
    [Authorize(Policy = "EmployeeInfo.View")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly EmailSender _emailSender;
        //private readonly AttendanceService _attendanceService;
        private ExcelProcess _excelProcess = new ExcelProcess();
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               IWebHostEnvironment hostEnvironment,
                               ApplicationDbContext context,
                               EmailSender emailSender)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
        }

        private string GetAvatarColor(string name)
        {
            var colors = new[] { "#ef4444", "#f97316", "#f59e0b", "#10b981", "#06b6d4", "#3b82f6", "#6366f1", "#8b5cf6", "#ec4899" };
            if (string.IsNullOrEmpty(name)) return colors[0];
            int hash = 0;
            foreach (char c in name) { hash += c; }
            return colors[hash % colors.Length];
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "NV";
            var words = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 1) return words[0].Substring(0, 1).ToUpper();
            return (words[0].Substring(0, 1) + words[words.Length - 1].Substring(0, 1)).ToUpper();
        }

        public async Task<IActionResult> Index()
        {
            var nhanSus = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ViTriCongViec)
                .Include(n => n.ThongTinCongViec)
                .ToListAsync();

            var employees = nhanSus.Select(nv => new EmployeeProfileVM
            {
                Id = nv.Id,
                MaNV = nv.Id,
                HoTen = nv.HoTen,
                Initials = GetInitials(nv.HoTen),
                AvatarColor = GetAvatarColor(nv.HoTen),
                AnhDaiDien = nv.AnhDaiDien,
                GioiTinh = string.IsNullOrEmpty(nv.GioiTinh) ? "-" : nv.GioiTinh,
                NgaySinh = nv.NgaySinh.HasValue ? nv.NgaySinh.Value.ToString("dd/MM/yyyy") : "-",
                DienThoai = string.IsNullOrEmpty(nv.SoDienThoai) ? "-" : nv.SoDienThoai,
                Email = string.IsNullOrEmpty(nv.Email) ? "-" : nv.Email,
                ViTri = nv.ViTriCongViec?.TenViTri ?? "Chưa cập nhật",
                PhongBan = nv.PhongBan?.TenPhongBan ?? "Chưa cập nhật",
                TrangThai = nv.TrangThai
            })
            .OrderBy(x => x.PhongBan)
            .ThenBy(x => x.HoTen)
            .ToList();

            ViewBag.DangLamViec = nhanSus.Count;
            ViewBag.ThuViec = nhanSus.Count(x => x.ThongTinCongViec != null && x.ThongTinCongViec.TinhChatLaoDong == "Thử việc");
            ViewBag.ChinhThuc = nhanSus.Count(x => x.ThongTinCongViec != null && x.ThongTinCongViec.TinhChatLaoDong == "Chính thức");
            ViewBag.NghiThaiSan = nhanSus.Count(x => x.ThongTinCongViec != null && x.ThongTinCongViec.TinhChatLaoDong == "Nghỉ thai sản");
            ViewBag.Khac = nhanSus.Count - (ViewBag.ThuViec + ViewBag.ChinhThuc + ViewBag.NghiThaiSan);
            ViewBag.SoNam = nhanSus.Count(x => x.GioiTinh == "Nam");
            ViewBag.SoNu = nhanSus.Count(x => x.GioiTinh == "Nữ");

            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var nhansu = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ThongTinChung)
                .Include(n => n.ThongTinLienHe)
                .Include(n => n.KinhNghiemLamViecs)
                .Include(n => n.ThongTinCongViec)
                .Include(n => n.ViTriCongViec)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nhansu == null) return NotFound();

            ViewBag.Initials = GetInitials(nhansu.HoTen);
            ViewBag.AvatarColor = GetAvatarColor(nhansu.HoTen);

            return View(nhansu);
        }

        [HttpGet]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public IActionResult Create()
        {
            ViewBag.ViTriList = new SelectList(_context.ViTriCongViecs, "Id", "TenViTri");
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");
            ViewBag.QuanLyList = new SelectList(_context.NhanSus, "Id", "HoTen");
            return View(new NhanSu());
        }

        [HttpPost]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> Create(SDHRM.Models.NhanSu model, IFormFile? LogoFile)
        {
            if (ModelState.IsValid)
            {
                var password = "Demo@1234";

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Tạo tài khoản ApplicationUser
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        PhoneNumber = model.SoDienThoai,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        // 2. Tạo đối tượng Nhân sự
                        var nhansu = new NhanSu
                        {
                            HoTen = model.HoTen,
                            NgaySinh = model.NgaySinh,
                            GioiTinh = model.GioiTinh,
                            DiaChi = model.DiaChi,
                            MaDinhDanh = model.MaDinhDanh,
                            PhongBanId = model.PhongBanId,
                            ViTriCongViecId = model.ViTriCongViecId,
                            QuanLyId = model.QuanLyId,
                            TrangThai = model.TrangThai,
                            Email = model.Email,
                            SoDienThoai = model.SoDienThoai,
                            UserId = user.Id
                        };

                        _context.NhanSus.Add(nhansu);

                        // 3. Xử lý ảnh đại diện
                        if (LogoFile != null && LogoFile.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(LogoFile.FileName);
                            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);

                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await LogoFile.CopyToAsync(stream);
                            }
                            nhansu.AnhDaiDien = "/uploads/" + uniqueFileName;
                        }

                        // 4. Lưu nhân sự vào DB để lấy ID
                        await _context.SaveChangesAsync();

                        // 5. Cập nhật lại EmployeeId cho User
                        user.EmployeeId = nhansu.Id;
                        await _userManager.UpdateAsync(user);

                        // NẾU TẤT CẢ ĐỀU ỔN TRAN CỦA TRÊN -> Chốt lưu toàn bộ vào DB
                        await transaction.CommitAsync();

                        // 6. Xử lý gửi mail (Bọc Try-Catch để nếu lỗi gửi mail thì vẫn báo tạo thành công)
                        try
                        {
                            string subject = "Tài khoản nhân viên của bạn";
                            string body = $@"
                            Xin chào {model.HoTen},<br/>
                            Bạn đã được tạo tài khoản nhân viên.<br/>
                            <b>Tài khoản:</b> {model.Email}<br/>
                            <b>Mật khẩu tạm thời:</b> {password}<br/>
                            Vui lòng đăng nhập và đổi mật khẩu sau khi sử dụng lần đầu.
                        ";
                            await _emailSender.SendEmailAsync(model.Email, subject, body);
                            TempData["Success"] = $"Tạo tài khoản thành công! Đã gửi email thông báo.";
                        }
                        catch (Exception ex)
                        {
                            // Mail lỗi nhưng dữ liệu đã lưu, báo cho người quản trị biết
                            TempData["Success"] = $"Tạo tài khoản thành công! Mật khẩu: {password} (Không thể gửi email tự động).";
                        }

                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    // Nếu có BẤT KỲ lỗi gì xảy ra (lỗi DB, lỗi file...), Rollback toàn bộ để DB sạch sẽ
                    await transaction.RollbackAsync();
                    string detailError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình xử lý: " + detailError);
                }
            }

            ViewBag.ViTriList = new SelectList(_context.ViTriCongViecs, "Id", "TenViTri");
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");
            ViewBag.QuanLyList = new SelectList(_context.NhanSus, "Id", "HoTen");

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> EditMember(int? id)
        {
            if (id == null) return NotFound();

            var nhansu = await _context.NhanSus.FindAsync(id);
            if (nhansu == null) return NotFound();

            // Tách Họ đệm và Tên để đẩy ra View
            string hoDem = "";
            string ten = "";
            if (!string.IsNullOrEmpty(nhansu.HoTen))
            {
                var parts = nhansu.HoTen.Trim().Split(' ');
                if (parts.Length > 0)
                {
                    ten = parts.Last(); // Chữ cuối cùng là Tên
                    if (parts.Length > 1)
                    {
                        hoDem = string.Join(" ", parts.Take(parts.Length - 1)); // Phần còn lại là Họ đệm
                    }
                }
            }
            ViewBag.HoDem = hoDem;
            ViewBag.Ten = ten;

            // Load các danh sách Dropdown (Loại trừ chính người này ra khỏi danh sách Quản lý để tránh tự quản lý mình)
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", nhansu.PhongBanId);
            ViewBag.ViTriList = new SelectList(_context.ViTriCongViecs, "Id", "TenViTri", nhansu.ViTriCongViecId);
            ViewBag.QuanLyList = new SelectList(_context.NhanSus.Where(n => n.Id != id), "Id", "HoTen", nhansu.QuanLyId);

            return View(nhansu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> EditMember(int id, SDHRM.Models.NhanSu model, IFormFile? LogoFile)
        {
            if (id != model.Id) return NotFound();

            // Xóa lỗi xác thực các trường liên kết
            ModelState.Remove("PhongBan");
            ModelState.Remove("ViTriCongViec");
            ModelState.Remove("QuanLy");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Lấy nhân sự cũ từ DB lên
                    var existingNhanSu = await _context.NhanSus.FindAsync(id);
                    if (existingNhanSu == null) return NotFound();

                    // Cập nhật các trường thông tin
                    existingNhanSu.HoTen = model.HoTen;
                    existingNhanSu.NgaySinh = model.NgaySinh;
                    existingNhanSu.GioiTinh = model.GioiTinh;
                    existingNhanSu.DiaChi = model.DiaChi;
                    existingNhanSu.MaDinhDanh = model.MaDinhDanh;
                    existingNhanSu.PhongBanId = model.PhongBanId;
                    existingNhanSu.ViTriCongViecId = model.ViTriCongViecId;
                    existingNhanSu.QuanLyId = model.QuanLyId;
                    existingNhanSu.TrangThai = model.TrangThai;
                    existingNhanSu.Email = model.Email;
                    existingNhanSu.SoDienThoai = model.SoDienThoai;

                    // Xử lý Ảnh đại diện (Nếu có up ảnh mới)
                    if (LogoFile != null && LogoFile.Length > 0)
                    {
                        // 1. Xóa ảnh cũ
                        if (!string.IsNullOrEmpty(existingNhanSu.AnhDaiDien))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingNhanSu.AnhDaiDien.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                        }

                        // 2. Lưu ảnh mới
                        var fileExtension = Path.GetExtension(LogoFile.FileName);
                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await LogoFile.CopyToAsync(stream);
                        }
                        existingNhanSu.AnhDaiDien = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(existingNhanSu);

                    // Cập nhật luôn Email và SĐT của User trong Identity (Nếu đã có tài khoản)
                    if (!string.IsNullOrEmpty(existingNhanSu.UserId))
                    {
                        var user = await _userManager.FindByIdAsync(existingNhanSu.UserId);
                        if (user != null)
                        {
                            user.Email = model.Email;
                            user.UserName = model.Email; // Thường UserName sẽ lấy bằng Email
                            user.PhoneNumber = model.SoDienThoai;
                            await _userManager.UpdateAsync(user);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Cập nhật thông tin đối tượng thành công!";
                    return RedirectToAction("Index"); 
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    string detailError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ModelState.AddModelError("", "Lỗi Database: " + detailError);
                }
            }

            // Nếu Form lỗi, nạp lại ViewBag
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", model.PhongBanId);
            ViewBag.ViTriList = new SelectList(_context.ViTriCongViecs, "Id", "TenViTri", model.ViTriCongViecId);
            ViewBag.QuanLyList = new SelectList(_context.NhanSus.Where(n => n.Id != id), "Id", "HoTen", model.QuanLyId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> ToggleStatusMember(int id)
        {
            var item = await _context.NhanSus.FindAsync(id); // Dùng trực tiếp _context.NhanSus cho gọn
            if (item == null) return NotFound();

            // Sửa lại string cho đúng với Database ("Đang làm việc" và "Nghỉ việc")
            item.TrangThai = (item.TrangThai == "Đang làm việc") ? "Nghỉ việc" : "Đang làm việc";

            _context.Update(item);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật trạng thái thành công!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeInfo.Delete")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            // 1. Tìm nhân viên cần xóa trong Database
            var nhansu = await _context.NhanSus.FindAsync(id);

            if (nhansu == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin nhân viên cần xóa!";
                return RedirectToAction("Index"); // Đổi "Member" thành tên hàm danh sách của bạn nếu khác
            }

            // Mở Transaction để đảm bảo an toàn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Tìm và xóa tài khoản ApplicationUser (Identity) trước
                if (!string.IsNullOrEmpty(nhansu.UserId))
                {
                    var user = await _userManager.FindByIdAsync(nhansu.UserId);
                    if (user != null)
                    {
                        var deleteUserResult = await _userManager.DeleteAsync(user);
                        if (!deleteUserResult.Succeeded)
                        {
                            // Nếu Identity báo lỗi không xóa được user, ném ra ngoại lệ để Rollback
                            throw new Exception("Hệ thống từ chối xóa tài khoản liên kết của nhân viên này.");
                        }
                    }
                }

                // 3. Xóa file ảnh đại diện trên máy chủ (Dọn rác ổ cứng)
                if (!string.IsNullOrEmpty(nhansu.AnhDaiDien))
                {
                    // Cắt bỏ dấu "/" ở đầu chuỗi (VD: "/uploads/anh.jpg" -> "uploads/anh.jpg")
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nhansu.AnhDaiDien.TrimStart('/'));

                    // Kiểm tra xem file có tồn tại thật không rồi mới xóa
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // 4. Xóa đối tượng Nhân sự
                _context.NhanSus.Remove(nhansu);
                await _context.SaveChangesAsync();

                // NẾU MỌI THỨ SUÔN SẺ -> Chốt (Commit) Transaction
                await transaction.CommitAsync();

                TempData["Success"] = $"Đã xóa thành công nhân viên {nhansu.HoTen} và tài khoản liên kết!";
            }
            catch (Exception ex)
            {
                // CÓ LỖI XẢY RA -> Quay xe (Rollback) khôi phục lại dữ liệu như cũ
                await transaction.RollbackAsync();
                TempData["Error"] = "Lỗi khi xóa nhân viên: " + ex.Message;
            }

            // Xong việc thì quay lại trang danh sách
            return RedirectToAction("Index");
        }
    }
}