using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Models;
using SDHRM.Data;
using Microsoft.AspNetCore.Authorization;

namespace SDHRM.Areas.Systems.Controllers
{
    [Area("Systems")]
    [Authorize(Policy = "Systems.Manage")]
    public class StructureController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StructureController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var model = await _context.CongTys.FirstOrDefaultAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SDHRM.Models.CongTy model, IFormFile? LogoFile)
        {
            // Lấy bản ghi đầu tiên (vì bảng chỉ có 1 công ty)
            var entity = await _context.CongTys.FirstOrDefaultAsync();

            if (entity == null)
            {
                // Nếu chưa có thì tạo mới (Optional)
                entity = new SDHRM.Models.CongTy();
                _context.CongTys.Add(entity);
            }

            // Cập nhật các trường thông tin
            entity.TenDayDu = model.TenDayDu;
            entity.TenVietTat = model.TenVietTat;
            entity.MaSoThue = model.MaSoThue;
            entity.NgayThanhLap = model.NgayThanhLap;

            entity.SoGiayPhepDKKD = model.SoGiayPhepDKKD;
            entity.NgayCap = model.NgayCap;
            entity.NoiCap = model.NoiCap;
            entity.DaiDienPhapLuat = model.DaiDienPhapLuat;

            entity.DiaChi = model.DiaChi;
            entity.SoDienThoai = model.SoDienThoai;
            entity.SoFax = model.SoFax;
            entity.Email = model.Email;
            entity.Website = model.Website;

            // Xử lý lưu file Logo (Nếu có chọn file mới)
            if (LogoFile != null && LogoFile.Length > 0)
            {
                // Lưu file vào thư mục wwwroot/uploads
                var fileName = Path.GetFileName(LogoFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                // Đảm bảo thư mục tồn tại
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(stream);
                }

                // Lưu đường dẫn vào DB
                entity.Logo = "/uploads/" + fileName;
            }

            // Lưu vào Database
            await _context.SaveChangesAsync();

            // Quay lại trang Index
            return RedirectToAction(nameof(Index));
        }
    }
}
