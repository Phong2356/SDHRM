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
