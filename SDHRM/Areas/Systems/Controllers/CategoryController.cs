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
using System;
using System.Threading.Tasks;

namespace SDHRM.Areas.Systems.Controllers
{
    [Area("Systems")]
    public class CategoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly EmailSender _emailSender;
        //private readonly AttendanceService _attendanceService;
        private ExcelProcess _excelProcess = new ExcelProcess();
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoryController(UserManager<ApplicationUser> userManager,
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

        // ------------------------ //
        private async Task<IActionResult> HandleCreate<T>(T entity, string redirectAction) where T : class, IBaseEntity
        {
            entity.TrangThai = "Đang hoạt động";
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(redirectAction);
            }
            return View(entity);
        }

        private async Task<IActionResult> HandleEdit<T>(int id, T entity, string redirectAction) where T : class, IBaseEntity
        {
            if (id != entity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(redirectAction);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Set<T>().Any(e => e.Id == id)) return NotFound();
                    throw;
                }
            }
            return View(entity);
        }

        private async Task<IActionResult> HandleDelete<T>(int id, string redirectAction) where T : class, IBaseEntity
        {
            var item = await _context.Set<T>().FindAsync(id);
            if (item == null) return NotFound();

            try
            {
                _context.Set<T>().Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa dữ liệu thành công!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Lỗi hệ thống: Không thể xóa bản ghi này.";
            }
            return RedirectToAction(redirectAction);
        }

        private async Task<IActionResult> HandleToggleStatus<T>(int id, string redirectAction) where T : class, IBaseEntity
        {
            var item = await _context.Set<T>().FindAsync(id);
            if (item == null) return NotFound();

            item.TrangThai = (item.TrangThai == "Đang hoạt động") ? "Ngừng hoạt động" : "Đang hoạt động";

            _context.Update(item);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật trạng thái thành công!";

            return RedirectToAction(redirectAction);
        }

        // ==========================================
        // 1. QUẢN LÝ Organization
        // ==========================================

        public async Task<IActionResult> Organization()
        {
            var allPhongBans = await _context.PhongBans
                                             .Include(p => p.TruongPhong)
                                             .ToListAsync();
            return View(allPhongBans);
        }

        public IActionResult CreateOrganization()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CreateOrganization(PhongBan phongban)
            => HandleCreate(phongban, nameof(Organization));
        
        public async Task<IActionResult> EditOrganization(int? id)
        {
            if (id == null) return NotFound();
            var organization = await _context.PhongBans.FindAsync(id);
            return organization == null ? NotFound() : View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> EditOrganization(int id, PhongBan phongban)
            => HandleEdit(id, phongban, nameof(Organization));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteOrganization(int id)
            => HandleDelete<PhongBan>(id, nameof(Organization));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ToggleStatusOrganization(int id)
            => HandleToggleStatus<PhongBan>(id, nameof(Organization));

        // ==========================================
        // 2. QUẢN LÝ Member
        // ==========================================

        public async Task<IActionResult> Member()
        {
            var nhanSus = await _context.NhanSus
                                      .Include(n => n.PhongBan)
                                      .Include(n => n.ViTriCongViec)
                                      .Include(n => n.QuanLy)
                                      .ToListAsync();
            return View(nhanSus);
        }

        [HttpGet]
        public IActionResult CreateMember()
        {
            ViewBag.ViTriList = new SelectList(_context.ViTriCongViecs, "Id", "TenViTri");
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");
            ViewBag.QuanLyList = new SelectList(_context.NhanSus, "Id", "HoTen");
            return View(new NhanSu());
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(SDHRM.Models.NhanSu model, IFormFile? LogoFile)
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

                        return RedirectToAction("Member");
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

        public async Task<IActionResult> DetailsMember(int? id)
        {
            if (id == null) return NotFound();

            var nhansu = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ViTriCongViec)
                .Include(n => n.QuanLy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nhansu == null) return NotFound();

            // CHÚ Ý: Đổi View thành PartialView
            return PartialView(nhansu);
        }

        // 1. HÀM GET: LẤY DỮ LIỆU CŨ HIỂN THỊ LÊN FORM
        [HttpGet]
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

        // 2. HÀM POST: LƯU DỮ LIỆU KHI BẤM NÚT LƯU
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    return RedirectToAction("Member"); // Sửa lại tên action danh sách của bạn nếu cần
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
        public async Task<IActionResult> ToggleStatusMember(int id)
        {
            var item = await _context.NhanSus.FindAsync(id); // Dùng trực tiếp _context.NhanSus cho gọn
            if (item == null) return NotFound();

            // Sửa lại string cho đúng với Database ("Đang làm việc" và "Nghỉ việc")
            item.TrangThai = (item.TrangThai == "Đang làm việc") ? "Nghỉ việc" : "Đang làm việc";

            _context.Update(item);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật trạng thái thành công!";

            return RedirectToAction("Member");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(int id)
        {
            // 1. Tìm nhân viên cần xóa trong Database
            var nhansu = await _context.NhanSus.FindAsync(id);

            if (nhansu == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin nhân viên cần xóa!";
                return RedirectToAction("Member"); // Đổi "Member" thành tên hàm danh sách của bạn nếu khác
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
            return RedirectToAction("Member");
        }

        // ==========================================
        // 3. QUẢN LÝ Jobposition
        // ==========================================

        public async Task<IActionResult> Jobposition()
            => View(await _context.ViTriCongViecs.ToListAsync());

        public IActionResult CreateJobposition() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CreateJobposition(ViTriCongViec vitricongviec)
            => HandleCreate(vitricongviec, nameof(Jobposition));

        public async Task<IActionResult> EditJobposition(int? id)
        {
            if (id == null) return NotFound();
            var jobposition = await _context.ViTriCongViecs.FindAsync(id);
            return jobposition == null ? NotFound() : View(jobposition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> EditJobposition(int id, ViTriCongViec vitricongviec)
            => HandleEdit(id, vitricongviec, nameof(Jobposition));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteJobposition(int id)
            => HandleDelete<ViTriCongViec>(id, nameof(Jobposition));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ToggleStatusJobposition(int id)
            => HandleToggleStatus<ViTriCongViec>(id, nameof(Jobposition));
    }
}
