using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using SDHRM.Data;
using SDHRM.Models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SDHRM.Areas.InfoEmployees.Controllers
{
    [Area("InfoEmployees")]
    [Authorize(Policy = "RewardsDisciplines.Manage")]
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RewardController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var khenThuongs = await _context.KhenThuongs
                .Include(k => k.PhongBan)
                .Include(k => k.NguoiQuyetDinh)
                .Include(k => k.ChiTietKhenThuongs) 
                .OrderByDescending(k => k.NgayTao)
                .ToListAsync();
            return View(khenThuongs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");

            var model = new KhenThuong
            {
                TenDotKhenThuong = "",
                LoaiKhenThuong = "",
                SoQuyetDinh = "",          
                DoiTuongKhenThuong = "Cá nhân",

                NgayKhenThuong = DateTime.Today,
                NgayQuyetDinh = DateTime.Today,
                TrangThai = "Chưa thực hiện"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhenThuong model, IFormFile? fileDinhKem)
        {
            ModelState.Remove("NguoiQuyetDinh"); ModelState.Remove("PhongBan");
            // Gỡ lỗi cho các trường có thể trống trong Chi tiết khen thưởng
            if (model.ChiTietKhenThuongs != null)
            {
                for (int i = 0; i < model.ChiTietKhenThuongs.Count; i++)
                {
                    ModelState.Remove($"ChiTietKhenThuongs[{i}].NhanSu");
                    ModelState.Remove($"ChiTietKhenThuongs[{i}].KhenThuong");
                }
            }

            if (ModelState.IsValid)
            {
                if (fileDinhKem != null && fileDinhKem.Length > 0)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "rewards");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    string fileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                    using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                        await fileDinhKem.CopyToAsync(stream);
                    model.TepDinhKem = fileName;
                }

                _context.Add(model);
                await _context.SaveChangesAsync(); // Entity Framework sẽ tự động lưu cả Đợt KT và Danh sách NV
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen", model.NguoiQuyetDinhId);
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", model.PhongBanId);
            return View(model);
        }
      
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Kéo Đợt khen thưởng kèm theo Danh sách nhân viên (và thông tin Vị trí, Phòng ban của họ)
            var khenThuong = await _context.KhenThuongs
                .Include(k => k.ChiTietKhenThuongs).ThenInclude(c => c.NhanSu).ThenInclude(n => n.ViTriCongViec)
                .Include(k => k.ChiTietKhenThuongs).ThenInclude(c => c.NhanSu).ThenInclude(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khenThuong == null) return NotFound();

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen", khenThuong.NguoiQuyetDinhId);
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", khenThuong.PhongBanId);

            return View(khenThuong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhenThuong model, IFormFile? fileDinhKem)
        {
            if (id != model.Id) return NotFound();

            ModelState.Remove("NguoiQuyetDinh"); ModelState.Remove("PhongBan");
            if (model.ChiTietKhenThuongs != null)
            {
                for (int i = 0; i < model.ChiTietKhenThuongs.Count; i++)
                {
                    ModelState.Remove($"ChiTietKhenThuongs[{i}].NhanSu");
                    ModelState.Remove($"ChiTietKhenThuongs[{i}].KhenThuong");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Lấy dữ liệu cũ từ DB (Bao gồm cả list nhân viên hiện tại)
                    var existingKhenThuong = await _context.KhenThuongs
                        .Include(k => k.ChiTietKhenThuongs)
                        .FirstOrDefaultAsync(k => k.Id == id);

                    if (existingKhenThuong == null) return NotFound();

                    // 2. Xóa list nhân viên cũ trong DB để dọn chỗ cho list mới từ form gửi lên
                    if (existingKhenThuong.ChiTietKhenThuongs != null)
                    {
                        _context.ChiTietKhenThuongs.RemoveRange(existingKhenThuong.ChiTietKhenThuongs);
                    }

                    // 3. Cập nhật thông tin Master (Đợt khen thưởng)
                    _context.Entry(existingKhenThuong).CurrentValues.SetValues(model);

                    // 4. Chèn list nhân viên mới vào
                    if (model.ChiTietKhenThuongs != null && model.ChiTietKhenThuongs.Any())
                    {
                        foreach (var detail in model.ChiTietKhenThuongs)
                        {
                            detail.Id = 0; // Đặt ID = 0 để EF hiểu là tạo mới record Chi tiết
                            detail.KhenThuongId = existingKhenThuong.Id;
                            existingKhenThuong.ChiTietKhenThuongs.Add(detail);
                        }
                    }

                    // 5. Xử lý File đính kèm
                    if (fileDinhKem != null && fileDinhKem.Length > 0)
                    {
                        string folder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "rewards");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        string fileName = Guid.NewGuid().ToString() + "_" + fileDinhKem.FileName;
                        using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                            await fileDinhKem.CopyToAsync(stream);
                        existingKhenThuong.TepDinhKem = fileName;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhenThuongExists(model.Id)) return NotFound();
                    else throw;
                }
            }

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen", model.NguoiQuyetDinhId);
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan", model.PhongBanId);
            return View(model);
        }

        private bool KhenThuongExists(int id)
        {
            return _context.KhenThuongs.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khenThuong = await _context.KhenThuongs
                .Include(k => k.PhongBan)
                .Include(k => k.NguoiQuyetDinh)
                .Include(k => k.ChiTietKhenThuongs).ThenInclude(c => c.NhanSu).ThenInclude(n => n.ViTriCongViec)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khenThuong == null) return NotFound();
            return View(khenThuong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var khenThuong = await _context.KhenThuongs.FindAsync(id);
            if (khenThuong != null)
            {
                if (!string.IsNullOrEmpty(khenThuong.TepDinhKem))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "rewards", khenThuong.TepDinhKem);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                _context.KhenThuongs.Remove(khenThuong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetChucDanh(int id)
        {
            var ns = await _context.NhanSus.Include(n => n.ViTriCongViec).FirstOrDefaultAsync(n => n.Id == id);
            return Json(new { chucDanh = ns?.ViTriCongViec?.TenViTri ?? "" });
        }

        [HttpGet]
        public async Task<IActionResult> GetNhanSuInfo(int id)
        {
            var ns = await _context.NhanSus.Include(n => n.ViTriCongViec).Include(n => n.PhongBan).FirstOrDefaultAsync(n => n.Id == id);
            return Json(new
            {
                maNV = ns?.Id,
                viTri = ns?.ViTriCongViec?.TenViTri ?? "",
                tenPhongBan = ns?.PhongBan?.TenPhongBan ?? ""
            });
        }
    }
}