using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy mã tài khoản (UserId) của người đang đăng nhập
            var currentUserId = _userManager.GetUserId(User);

            // Nếu chưa đăng nhập thì đẩy ra trang Login
            if (currentUserId == null)
            {
                return Challenge();
            }

            // 2. Tự động móc đúng hồ sơ của người đó lên
            var nhansu = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ThongTinChung)
                .Include(n => n.ThongTinLienHe)
                .Include(n => n.KinhNghiemLamViecs)
                .Include(n => n.ThongTinCongViec)
                .Include(n => n.ViTriCongViec)
                .Include(n => n.BangCaps)
                .Include(n => n.ChungChis)
                .Include(n => n.ChiTietKhenThuongs)
                    .ThenInclude(ct => ct.KhenThuong)
                .Include(n => n.ChiTietKyLuats)
                    .ThenInclude(ct => ct.KyLuat)
                .Include(n => n.QuaTrinhCongTacs)
                    .ThenInclude(q => q.PhongBan)
                .Include(n => n.QuaTrinhCongTacs)
                    .ThenInclude(q => q.ViTriCongViec)
                .FirstOrDefaultAsync(m => m.UserId == currentUserId);

            // 3. Nếu tài khoản này chưa được bộ phận nhân sự tạo liên kết thông tin
            if (nhansu == null)
            {
                return NotFound("Hồ sơ của bạn chưa được thiết lập trên hệ thống!");
            }

            // 4. Trả về View Index.cshtml
            return View(nhansu);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int? id)
        {
            if (id == null) return NotFound();

            var nhansu = await _context.NhanSus
                .Include(n => n.ThongTinChung)
                .Include(n => n.ThongTinLienHe)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nhansu == null) return NotFound();

            // Khởi tạo nếu rỗng để tránh lỗi khi render View
            if (nhansu.ThongTinChung == null) nhansu.ThongTinChung = new ThongTinChung();
            if (nhansu.ThongTinLienHe == null) nhansu.ThongTinLienHe = new ThongTinLienHe();

            return View(nhansu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(int id, NhanSu model)
        {
            if (id != model.Id) return NotFound();

            // Bỏ qua validate các trường không cần thiết trong form này (Tránh bị ModelState.IsValid = false)
            ModelState.Remove("PhongBan");
            ModelState.Remove("ViTriCongViec");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi gốc từ DB
                    var nhansuDb = await _context.NhanSus
                        .Include(n => n.ThongTinChung)
                        .Include(n => n.ThongTinLienHe)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (nhansuDb == null) return NotFound();

                    // 1. Cập nhật bảng NhanSu
                    nhansuDb.HoTen = model.HoTen;
                    nhansuDb.NgaySinh = model.NgaySinh;
                    nhansuDb.GioiTinh = model.GioiTinh;
                    nhansuDb.SoDienThoai = model.SoDienThoai;
                    nhansuDb.Email = model.Email;

                    // 2. Cập nhật bảng ThongTinChung
                    if (nhansuDb.ThongTinChung == null) nhansuDb.ThongTinChung = new ThongTinChung();
                    nhansuDb.ThongTinChung.NoiSinh = model.ThongTinChung?.NoiSinh;
                    nhansuDb.ThongTinChung.NguyenQuan = model.ThongTinChung?.NguyenQuan;
                    nhansuDb.ThongTinChung.TinhTrangHonNhan = model.ThongTinChung?.TinhTrangHonNhan;
                    nhansuDb.ThongTinChung.MaSoThue = model.ThongTinChung?.MaSoThue;
                    nhansuDb.ThongTinChung.DanToc = model.ThongTinChung?.DanToc;
                    nhansuDb.ThongTinChung.TonGiao = model.ThongTinChung?.TonGiao;
                    nhansuDb.ThongTinChung.QuocTich = model.ThongTinChung?.QuocTich;

                    nhansuDb.ThongTinChung.SoCCCD = model.ThongTinChung?.SoCCCD;
                    nhansuDb.ThongTinChung.NgayCapGiayTo = model.ThongTinChung?.NgayCapGiayTo;
                    nhansuDb.ThongTinChung.NoiCapGiayTo = model.ThongTinChung?.NoiCapGiayTo;
                    nhansuDb.ThongTinChung.NgayHetHanGiayTo = model.ThongTinChung?.NgayHetHanGiayTo;

                    nhansuDb.ThongTinChung.SoHoChieu = model.ThongTinChung?.SoHoChieu;
                    nhansuDb.ThongTinChung.NgayCapHoChieu = model.ThongTinChung?.NgayCapHoChieu;
                    nhansuDb.ThongTinChung.NoiCapHoChieu = model.ThongTinChung?.NoiCapHoChieu;
                    nhansuDb.ThongTinChung.NgayHetHanHoChieu = model.ThongTinChung?.NgayHetHanHoChieu;

                    nhansuDb.ThongTinChung.TrinhDoHocVan = model.ThongTinChung?.TrinhDoHocVan;
                    nhansuDb.ThongTinChung.TrinhDoDaoTao = model.ThongTinChung?.TrinhDoDaoTao;
                    nhansuDb.ThongTinChung.NoiDaoTao = model.ThongTinChung?.NoiDaoTao;
                    nhansuDb.ThongTinChung.Khoa = model.ThongTinChung?.Khoa;
                    nhansuDb.ThongTinChung.ChuyenNganh = model.ThongTinChung?.ChuyenNganh;
                    nhansuDb.ThongTinChung.NamTotNghiep = model.ThongTinChung?.NamTotNghiep;
                    nhansuDb.ThongTinChung.XepLoaiTotNghiep = model.ThongTinChung?.XepLoaiTotNghiep;

                    // 3. Cập nhật bảng ThongTinLienHe
                    if (nhansuDb.ThongTinLienHe == null) nhansuDb.ThongTinLienHe = new ThongTinLienHe();
                    nhansuDb.ThongTinLienHe.DTCoQuan = model.ThongTinLienHe?.DTCoQuan;
                    nhansuDb.ThongTinLienHe.DTNhaRieng = model.ThongTinLienHe?.DTNhaRieng;
                    nhansuDb.ThongTinLienHe.DTKhac = model.ThongTinLienHe?.DTKhac;
                    nhansuDb.ThongTinLienHe.EmailCoQuan = model.ThongTinLienHe?.EmailCoQuan;
                    nhansuDb.ThongTinLienHe.EmailKhac = model.ThongTinLienHe?.EmailKhac;
                    nhansuDb.ThongTinLienHe.LinkMangXaHoi = model.ThongTinLienHe?.LinkMangXaHoi;

                    nhansuDb.ThongTinLienHe.DiaChiThuongTru = model.ThongTinLienHe?.DiaChiThuongTru;
                    nhansuDb.ThongTinLienHe.DiaChiTamTru = model.ThongTinLienHe?.DiaChiTamTru;

                    nhansuDb.ThongTinLienHe.HTLienHeKhanCap = model.ThongTinLienHe?.HTLienHeKhanCap;
                    nhansuDb.ThongTinLienHe.DTLienHeKhanCap = model.ThongTinLienHe?.DTLienHeKhanCap;
                    nhansuDb.ThongTinLienHe.EmailLienHeKhanCap = model.ThongTinLienHe?.EmailLienHeKhanCap;
                    nhansuDb.ThongTinLienHe.DiaChiLienHeKhanCap = model.ThongTinLienHe?.DiaChiLienHeKhanCap;

                    _context.Update(nhansuDb);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanSuExists(model.Id)) return NotFound();
                    else throw;
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AddBangCap(int nhanSuId)
        {
            var model = new BangCap { NhanSuId = nhanSuId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBangCap(BangCap model, IFormFile? fileDinhKem)
        {
            // Bỏ qua validate các trường ảo (nếu có)
            ModelState.Remove("NhanSu");

            if (ModelState.IsValid)
            {
                // Xử lý upload file đính kèm (nếu có)
                if (fileDinhKem != null && fileDinhKem.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "bangcap");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileDinhKem.CopyToAsync(fileStream);
                    }
                    model.TepDinhKem = uniqueFileName; // Lưu tên file vào Database
                }

                _context.BangCaps.Add(model);
                await _context.SaveChangesAsync();

                // Quay lại trang Profile sau khi lưu thành công
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AddChungChi(int nhanSuId)
        {
            var model = new ChungChi { NhanSuId = nhanSuId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChungChi(ChungChi model, IFormFile? fileDinhKem)
        {
            ModelState.Remove("NhanSu");
            if (ModelState.IsValid)
            {
                if (fileDinhKem != null && fileDinhKem.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chungchi");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                    using (var fileStream = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                        await fileDinhKem.CopyToAsync(fileStream);
                    model.TepDinhKem = uniqueFileName;
                }
                _context.ChungChis.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AddKinhNghiem(int nhanSuId)
        {
            var model = new KinhNghiemLamViec
            {
                NhanSuId = nhanSuId,
                ThoiGianBatDau = DateTime.Now,
                NoiLamViec = "",       
                ViTriCongViec = ""    
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddKinhNghiem(KinhNghiemLamViec model)
        {
            ModelState.Remove("NhanSu");
            if (ModelState.IsValid)
            {
                _context.KinhNghiemLamViecs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        private bool NhanSuExists(int id)
        {
            return _context.NhanSus.Any(e => e.Id == id);
        }
    }
}