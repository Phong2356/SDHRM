using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "EmployeeInfo.View")]
    public class BenefitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BenefitController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var phucLois = await _context.PhucLois
                .Include(p => p.PhucLoiNhanViens)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();
            return View(phucLois);
        }

        [HttpGet]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public IActionResult Create()
        {
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");

            var model = new PhucLoi
            {
                TenChuongTrinh = "",
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddDays(1),
                TrangThai = "Chưa thực hiện"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> Create(PhucLoi model)
        {       
            if (model.PhucLoiNhanViens != null)
            {
                for (int i = 0; i < model.PhucLoiNhanViens.Count; i++)
                {
                    ModelState.Remove($"PhucLoiNhanViens[{i}].NhanSu");
                    ModelState.Remove($"PhucLoiNhanViens[{i}].PhucLoi");
                }
            }

            if (model.PhucLoiChiPhis != null)
            {
                for (int i = 0; i < model.PhucLoiChiPhis.Count; i++)
                {
                    ModelState.Remove($"PhucLoiChiPhis[{i}].PhucLoi");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var phucLoi = await _context.PhucLois.FindAsync(id);
            if (phucLoi != null)
            {
                _context.PhucLois.Remove(phucLoi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var phucLoi = await _context.PhucLois
                .Include(p => p.PhucLoiNhanViens).ThenInclude(pn => pn.NhanSu).ThenInclude(ns => ns.ViTriCongViec)
                .Include(p => p.PhucLoiNhanViens).ThenInclude(pn => pn.NhanSu).ThenInclude(ns => ns.PhongBan)
                .Include(p => p.PhucLoiChiPhis)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (phucLoi == null) return NotFound();
            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");

            return View(phucLoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> Edit(int id, PhucLoi model)
        {
            if (id != model.Id) return NotFound();

            ModelState.Remove("PhongBan");
            if (model.PhucLoiNhanViens != null)
            {
                for (int i = 0; i < model.PhucLoiNhanViens.Count; i++)
                {
                    ModelState.Remove($"PhucLoiNhanViens[{i}].NhanSu");
                    ModelState.Remove($"PhucLoiNhanViens[{i}].PhucLoi");
                }
            }
            if (model.PhucLoiChiPhis != null)
            {
                for (int i = 0; i < model.PhucLoiChiPhis.Count; i++)
                {
                    ModelState.Remove($"PhucLoiChiPhis[{i}].PhucLoi");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPhucLoi = await _context.PhucLois
                        .Include(p => p.PhucLoiNhanViens)
                        .Include(p => p.PhucLoiChiPhis)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingPhucLoi == null) return NotFound();

                    // 1. Xóa danh sách cũ
                    if (existingPhucLoi.PhucLoiNhanViens != null)
                        _context.PhucLoiNhanViens.RemoveRange(existingPhucLoi.PhucLoiNhanViens);

                    if (existingPhucLoi.PhucLoiChiPhis != null)
                        _context.PhucLoiChiPhis.RemoveRange(existingPhucLoi.PhucLoiChiPhis);

                    // 2. Cập nhật thông tin Master
                    _context.Entry(existingPhucLoi).CurrentValues.SetValues(model);

                    // 3. Thêm danh sách mới từ Form gửi lên
                    if (model.PhucLoiNhanViens != null)
                    {
                        foreach (var nv in model.PhucLoiNhanViens)
                        {
                            nv.Id = 0; nv.PhucLoiId = existingPhucLoi.Id;
                            existingPhucLoi.PhucLoiNhanViens.Add(nv);
                        }
                    }
                    if (model.PhucLoiChiPhis != null)
                    {
                        foreach (var cp in model.PhucLoiChiPhis)
                        {
                            cp.Id = 0; cp.PhucLoiId = existingPhucLoi.Id;
                            existingPhucLoi.PhucLoiChiPhis.Add(cp);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PhucLois.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
            }

            ViewBag.NhanSuList = new SelectList(_context.NhanSus, "Id", "HoTen");
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var phucLoi = await _context.PhucLois
                .Include(p => p.PhucLoiNhanViens)
                    .ThenInclude(pn => pn.NhanSu)
                    .ThenInclude(ns => ns.ViTriCongViec)
                .Include(p => p.PhucLoiChiPhis)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (phucLoi == null) return NotFound();
            return View(phucLoi);
        }

        [HttpGet]
        [Authorize(Policy = "EmployeeInfo.Manage")]
        public async Task<IActionResult> GetNhanSuInfo(int id)
        {
            var ns = await _context.NhanSus
                .Include(n => n.ViTriCongViec)
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (ns == null) return NotFound();

            return Json(new
            {
                maNV =ns.Id,
                viTri = ns.ViTriCongViec?.TenViTri ?? "Chưa cập nhật",
                tenPhongBan = ns.PhongBan?.TenPhongBan ?? "Chưa cập nhật"
            });
        }
    }
}