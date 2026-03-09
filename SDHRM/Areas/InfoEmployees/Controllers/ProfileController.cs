using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using SDHRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDHRM.Areas.InfoEmployees.Controllers
{
    [Area("InfoEmployees")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        // Khởi tạo DbContext để móc dữ liệu
        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private string GetAvatarColor(string name)
        {
            var colors = new[] { "#ef4444", "#f97316", "#f59e0b", "#10b981", "#06b6d4", "#3b82f6", "#6366f1", "#8b5cf6", "#ec4899" };
            if (string.IsNullOrEmpty(name)) return colors[0];
            int hash = 0;
            foreach (char c in name) { hash += c; }
            return colors[hash % colors.Length];
        }
        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "NV";
            var words = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 1) return words[0].Substring(0, 1).ToUpper();
            return (words[0].Substring(0, 1) + words[words.Length - 1].Substring(0, 1)).ToUpper();
        }


        public async Task<IActionResult> Index()
        {
            var nhanSus = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ViTriCongViec)
                .Include(n => n.ThongTinCongViec)
                .ToListAsync();

            var employees = nhanSus.Select(nv => new EmployeeProfileVM
            {
                Id = nv.Id,
                MaNV =  nv.Id,
                HoTen = nv.HoTen,
                Initials = GetInitials(nv.HoTen),
                AvatarColor = GetAvatarColor(nv.HoTen),
                AnhDaiDien = nv.AnhDaiDien,
                GioiTinh = string.IsNullOrEmpty(nv.GioiTinh) ? "-" : nv.GioiTinh,
                NgaySinh = nv.NgaySinh.HasValue ? nv.NgaySinh.Value.ToString("dd/MM/yyyy") : "-",
                DienThoai = string.IsNullOrEmpty(nv.SoDienThoai) ? "-" : nv.SoDienThoai,
                Email = string.IsNullOrEmpty(nv.Email) ? "-" : nv.Email,
                ViTri = nv.ViTriCongViec?.TenViTri ?? "Chưa cập nhật",
                PhongBan = nv.PhongBan?.TenPhongBan ?? "Chưa cập nhật"
            })
            .OrderBy(x => x.PhongBan)
            .ThenBy(x => x.HoTen)
            .ToList();

            // 3. Tính toán số liệu thống kê cho các thẻ (Cards) ở trên cùng
            ViewBag.DangLamViec = nhanSus.Count;

            // LƯU Ý: Nếu cột trạng thái trong DB của bạn tên khác, hãy sửa chữ TrangThai thành tên cột tương ứng
            ViewBag.ThuViec = nhanSus.Count(x => x.ThongTinCongViec != null && x.ThongTinCongViec.TinhChatLaoDong == "Thử việc");
            ViewBag.ChinhThuc = nhanSus.Count(x => x.ThongTinCongViec != null && x.ThongTinCongViec.TinhChatLaoDong == "Chính thức");
            ViewBag.NghiThaiSan = nhanSus.Count(x => x.ThongTinCongViec != null && x.ThongTinCongViec.TinhChatLaoDong == "Nghỉ thai sản");
            ViewBag.Khac = nhanSus.Count - (ViewBag.ThuViec + ViewBag.ChinhThuc + ViewBag.NghiThaiSan);

            // 4. Thống kê Giới tính cho Biểu đồ tròn
            ViewBag.SoNam = nhanSus.Count(x => x.GioiTinh == "Nam");
            ViewBag.SoNu = nhanSus.Count(x => x.GioiTinh == "Nữ");

            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
             
            var nhansu = await _context.NhanSus
                .Include(n => n.PhongBan)
                .Include(n => n.ThongTinChung)
                .Include(n => n.ThongTinLienHe)
                .Include(n => n.KinhNghiemLamViecs)
                .Include(n => n.ThongTinCongViec)
                .Include(n => n.ViTriCongViec)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nhansu == null)
            {
                return NotFound();
            }

            // Gửi màu sắc và chữ cái Avatar ra View
            ViewBag.Initials = GetInitials(nhansu.HoTen);
            ViewBag.AvatarColor = GetAvatarColor(nhansu.HoTen);

            // 4. Trả về View Index.cshtml
            return View(nhansu);
        }
    }
}