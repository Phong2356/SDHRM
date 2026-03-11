using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Authorize(Policy = "Payroll.Manage")]
    public class SalarycompositionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalarycompositionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.ThanhPhanLuongs
                .OrderBy(t => t.MaThanhPhan)
                .ToListAsync();

            return View(danhSach);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PhongBanList = new SelectList(_context.PhongBans, "Id", "TenPhongBan");

            var model = new ThanhPhanLuong
            {
                MaThanhPhan = "",
                TenThanhPhan = ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhPhanLuong model)
        {
            ModelState.Remove("PhongBan"); // Bỏ qua validate khóa ngoại
            if (ModelState.IsValid)
            {
                _context.ThanhPhanLuongs.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var model = await _context.ThanhPhanLuongs.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThanhPhanLuong model)
        {
            if (id != model.Id) return NotFound();
            ModelState.Remove("PhongBan");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ThanhPhanLuongs.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.ThanhPhanLuongs.FindAsync(id);
            if (model != null)
            {
                _context.ThanhPhanLuongs.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
