using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Authorize(Policy = "Payroll.View")]
    public class SalarydataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalarydataController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Timekeeping()
        {          
            var danhSachDaChot = await _context.BangChamCongs
                .Where(b => b.TrangThai == "Đã khóa")
                .OrderByDescending(b => b.TuNgay)
                .ToListAsync();

            return View(danhSachDaChot);
        }

        public async Task<IActionResult> TimekeepingDetail(int id)
        {
            var bangChamCong = await _context.BangChamCongs.FirstOrDefaultAsync(b => b.Id == id);
            if (bangChamCong == null || bangChamCong.TrangThai != "Đã khóa") return NotFound();

            var danhSachTongHop = await _context.BangChamCongNhanViens
                .Include(b => b.NhanSu)
                .Where(b => b.BangChamCongId == id)
                .ToListAsync();

            ViewBag.BangChamCong = bangChamCong;
            return View(danhSachTongHop);
        }

        public async Task<IActionResult> Salaryprofile()
        {
            var danhSach = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.HoSoLuong)
                .OrderBy(n => n.Id)
                .ToListAsync();

            return View(danhSach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Manage")]
        public async Task<IActionResult> CapNhatNhanh(int NhanSuId, decimal LuongCoBan, decimal LuongDongBaoHiem, int SoNguoiPhuThuoc, decimal PhuCapChucVu, decimal PhuCapDiLai, decimal PhuCapKhac)
        {
            var hoSo = await _context.HoSoLuongs.FirstOrDefaultAsync(h => h.NhanSuId == NhanSuId);

            if (hoSo == null)
            {
                hoSo = new HoSoLuong
                {
                    NhanSuId = NhanSuId,
                    LuongCoBan = LuongCoBan,
                    LuongDongBaoHiem = LuongDongBaoHiem,
                    SoNguoiPhuThuoc = SoNguoiPhuThuoc,
                    PhuCapChucVu = PhuCapChucVu, 
                    PhuCapDiLai = PhuCapDiLai,   
                    PhuCapKhac = PhuCapKhac,    
                    NgayCapNhat = DateTime.Now
                };
                _context.HoSoLuongs.Add(hoSo);
            }
            else
            {
                hoSo.LuongCoBan = LuongCoBan;
                hoSo.LuongDongBaoHiem = LuongDongBaoHiem;
                hoSo.SoNguoiPhuThuoc = SoNguoiPhuThuoc;
                hoSo.PhuCapChucVu = PhuCapChucVu;
                hoSo.PhuCapDiLai = PhuCapDiLai;   
                hoSo.PhuCapKhac = PhuCapKhac;    
                hoSo.NgayCapNhat = DateTime.Now;
                _context.HoSoLuongs.Update(hoSo);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật hồ sơ lương thành công!";

            return RedirectToAction(nameof(Salaryprofile));
        }
    }
}
