using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SDHRM.Areas.InfoEmployees.Controllers
{
    [Area("InfoEmployees")]
    public class ContractController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; 

        public ContractController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetNhanSuInfo(int id)
        {
            var nhanSu = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ViTriCongViec)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nhanSu == null)
            {
                return NotFound();
            }

            // Trả về dữ liệu dạng JSON
            return Json(new
            {
                maNV = nhanSu.Id,
                phongBanId = nhanSu.PhongBanId,
                viTri = nhanSu.ViTriCongViec?.TenViTri ?? "Chưa cập nhật"
            });
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Now.Date;
            var thirtyDaysLater = today.AddDays(30);

            // Lấy danh sách hợp đồng kèm thông tin nhân sự
            var hopDongs = await _context.HopDongs
                .Include(h => h.NhanSu).ThenInclude(n => n.ViTriCongViec)
                .Include(h => h.PhongBan)
                .OrderByDescending(h => h.NgayKy)
                .ToListAsync();

            // Tính toán thẻ thống kê (Top Cards)
            ViewBag.DangCoHieuLuc = hopDongs.Count(h => h.NgayCoHieuLuc.Date <= today && h.NgayHetHan.Date >= today);

            // Đếm số nhân sự chưa có hợp đồng nào
            var nhanSuCoHopDongIds = hopDongs.Select(h => h.NhanSuId).Distinct();
            ViewBag.ChuaCoHopDong = await _context.NhanSus.CountAsync(n => !nhanSuCoHopDongIds.Contains(n.Id));

            ViewBag.SapHetHan = hopDongs.Count(h => h.NgayHetHan.Date > today && h.NgayHetHan.Date <= thirtyDaysLater);
            ViewBag.HetHanChuaTaiKy = hopDongs.Count(h => h.NgayHetHan.Date < today);

            return View(hopDongs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");

            // Lấy số hợp đồng
            var lastContract = _context.HopDongs.OrderByDescending(h => h.Id).FirstOrDefault();
            int nextId = (lastContract?.Id ?? 0) + 1;
            string soHopDongMoi = "HD" + nextId.ToString("D6");

            // GÁN TRỰC TIẾP SỐ HỢP ĐỒNG VÀO MODEL THAY VÌ DÙNG VIEWBAG
            var model = new HopDong
            {
                SoHopDong = soHopDongMoi,
                TyLeHuongLuong = 100,
                NgayKy = DateTime.Today,
                NgayCoHieuLuc = DateTime.Today,
                NgayHetHan = DateTime.Today.AddYears(1)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HopDong hopDong, IFormFile? fileDinhKem)
        {
            // 1. Gỡ bỏ kiểm tra các trường khóa ngoại
            ModelState.Remove("NhanSu");
            ModelState.Remove("PhongBan");
            ModelState.Remove("NguoiDaiDien");

            // 2. GỠ BỎ KIỂM TRA CÁC TRƯỜNG STRING CÓ THỂ ĐỂ TRỐNG
            // (Đây là "thủ phạm" chính khiến bạn không lưu được)
            ModelState.Remove("TepDinhKem");
            ModelState.Remove("GhiChu");
            ModelState.Remove("TenHopDong");
            ModelState.Remove("ChucDanhNguoiDaiDien");
            ModelState.Remove("TrichYeu");

            if (ModelState.IsValid)
            {
                // Xử lý Upload file đính kèm
                if (fileDinhKem != null && fileDinhKem.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "contracts");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileDinhKem.CopyToAsync(fileStream);
                    }
                    hopDong.TepDinhKem = uniqueFileName;
                }

                _context.Add(hopDong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu form lỗi, load lại Dropdown
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen", hopDong.NhanSuId);
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", hopDong.PhongBanId);
            return View(hopDong);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen", hopDong.NhanSuId);
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", hopDong.PhongBanId);

            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HopDong hopDong, IFormFile? fileDinhKem)
        {
            if (id != hopDong.Id) return NotFound();

            ModelState.Remove("NhanSu");
            ModelState.Remove("PhongBan");
            ModelState.Remove("NguoiDaiDien");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy hợp đồng cũ từ DB ra để giữ lại file cũ nếu người dùng không upload file mới
                    var oldHopDong = await _context.HopDongs.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);

                    if (fileDinhKem != null && fileDinhKem.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "contracts");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileDinhKem.CopyToAsync(fileStream);
                        }
                        hopDong.TepDinhKem = uniqueFileName;
                    }
                    else
                    {
                        // Nếu không chọn file mới, giữ nguyên file cũ
                        hopDong.TepDinhKem = oldHopDong.TepDinhKem;
                    }

                    _context.Update(hopDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HopDongExists(hopDong.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen", hopDong.NhanSuId);
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", hopDong.PhongBanId);
            return View(hopDong);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hopDong = await _context.HopDongs
                .Include(h => h.NhanSu).ThenInclude(n => n.ViTriCongViec)
                .Include(h => h.PhongBan)
                .Include(h => h.NguoiDaiDien)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hopDong == null) return NotFound();

            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong != null)
            {
                // 1. Tìm và xóa file vật lý đính kèm (nếu có) để giải phóng dung lượng Server
                if (!string.IsNullOrEmpty(hopDong.TepDinhKem))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "contracts", hopDong.TepDinhKem);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // 2. Xóa dữ liệu trong Database
                _context.HopDongs.Remove(hopDong);
                await _context.SaveChangesAsync();
            }

            // Xóa xong thì load lại trang danh sách
            return RedirectToAction(nameof(Index));
        }

        private bool HopDongExists(int id)
        {
            return _context.HopDongs.Any(e => e.Id == id);
        }
    }
}