using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SDHRM.Areas.InfoEmployees.Controllers
{
    [Area("InfoEmployees")]
    public class DisciplineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DisciplineController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var kyLuats = await _context.KyLuats
                .Include(k => k.PhongBan)
                .Include(k => k.ChiTietKyLuats)
                .OrderByDescending(k => k.NgayTao)
                .ToListAsync();
            return View(kyLuats);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");

            var model = new KyLuat
            {
                TenKyLuat = "",
                LoaiKyLuat = "",
                NgayXayRa = DateTime.Today,
                TrangThai = "Chưa xử lý"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KyLuat model, IFormFile? fileDinhKem)
        {
            ModelState.Remove("PhongBan");

            if (model.ChiTietKyLuats != null)
            {
                for (int i = 0; i < model.ChiTietKyLuats.Count; i++)
                {
                    ModelState.Remove($"ChiTietKyLuats[{i}].NhanSu");
                    ModelState.Remove($"ChiTietKyLuats[{i}].KyLuat");
                }
            }

            if (ModelState.IsValid)
            {
                if (fileDinhKem != null && fileDinhKem.Length > 0)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "disciplines");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    string fileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                    using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                        await fileDinhKem.CopyToAsync(stream);
                    model.TepDinhKem = fileName;
                }

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", model.PhongBanId);
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var kyLuat = await _context.KyLuats
                .Include(k => k.ChiTietKyLuats).ThenInclude(c => c.NhanSu).ThenInclude(n => n.ViTriCongViec)
                .Include(k => k.ChiTietKyLuats).ThenInclude(c => c.NhanSu).ThenInclude(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kyLuat == null) return NotFound();

            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", kyLuat.PhongBanId);
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");

            return View(kyLuat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KyLuat model, IFormFile? fileDinhKem)
        {
            if (id != model.Id) return NotFound();

            ModelState.Remove("PhongBan");
            if (model.ChiTietKyLuats != null)
            {
                for (int i = 0; i < model.ChiTietKyLuats.Count; i++)
                {
                    ModelState.Remove($"ChiTietKyLuats[{i}].NhanSu");
                    ModelState.Remove($"ChiTietKyLuats[{i}].KyLuat");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingKyLuat = await _context.KyLuats
                        .Include(k => k.ChiTietKyLuats)
                        .FirstOrDefaultAsync(k => k.Id == id);

                    if (existingKyLuat == null) return NotFound();

                    // Xóa danh sách nhân viên vi phạm cũ để thay bằng danh sách mới
                    if (existingKyLuat.ChiTietKyLuats != null)
                    {
                        _context.ChiTietKyLuats.RemoveRange(existingKyLuat.ChiTietKyLuats);
                    }

                    _context.Entry(existingKyLuat).CurrentValues.SetValues(model);

                    if (model.ChiTietKyLuats != null && model.ChiTietKyLuats.Any())
                    {
                        foreach (var detail in model.ChiTietKyLuats)
                        {
                            detail.Id = 0;
                            detail.KyLuatId = existingKyLuat.Id;
                            existingKyLuat.ChiTietKyLuats.Add(detail);
                        }
                    }

                    if (fileDinhKem != null && fileDinhKem.Length > 0)
                    {
                        string folder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "disciplines");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        string fileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                        using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                            await fileDinhKem.CopyToAsync(stream);
                        existingKyLuat.TepDinhKem = fileName;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KyLuatExists(model.Id)) return NotFound();
                    else throw;
                }
            }

            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", model.PhongBanId);
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var kyLuat = await _context.KyLuats
                .Include(k => k.PhongBan)
                .Include(k => k.ChiTietKyLuats).ThenInclude(c => c.NhanSu).ThenInclude(n => n.ViTriCongViec)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kyLuat == null) return NotFound();
            return View(kyLuat);
        }

        private bool KyLuatExists(int id) { return _context.KyLuats.Any(e => e.Id == id); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kyLuat = await _context.KyLuats.FindAsync(id);
            if (kyLuat != null)
            {
                if (!string.IsNullOrEmpty(kyLuat.TepDinhKem))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "disciplines", kyLuat.TepDinhKem);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                _context.KyLuats.Remove(kyLuat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetNhanSuInfo(int id)
        {
            var ns = await _context.NhanSus
                .Include(n => n.ViTriCongViec)
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(n => n.Id == id);

            // Xử lý an toàn: Nếu không tìm thấy thì báo lỗi 404
            if (ns == null)
            {
                return NotFound();
            }

            return Json(new
            {
                maNV = ns.Id,
                viTri = ns.ViTriCongViec?.TenViTri ?? "Chưa cập nhật",
                tenPhongBan = ns.PhongBan?.TenPhongBan ?? "Chưa cập nhật"
            });
        }
    }
}