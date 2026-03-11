using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using System.Data;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Authorize(Policy = "Payroll.View")]
    public class SalaryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.BangLuongs
                .Include(b => b.BangChamCong)
                .Include(b => b.MauBangLuong) // Load thêm tên mẫu
                .OrderByDescending(b => b.NgayTao)
                .ToListAsync();

            ViewBag.BangChamCongList = new SelectList(_context.BangChamCongs.Where(b => b.TrangThai == "Đã khóa"), "Id", "TenBangChamCong");
            // Truyền danh sách Mẫu bảng lương ra Popup
            ViewBag.MauBangLuongList = new SelectList(_context.MauBangLuongs, "Id", "TenMau");
            return View(danhSach);
        }

        // 2. THUẬT TOÁN CHẠY LƯƠNG ĐỘNG
        [HttpPost]
        [Authorize(Policy = "Payroll.Manage")]
        public async Task<IActionResult> Create(string TenBangLuong, int BangChamCongId, int MauBangLuongId, int Thang, int Nam)
        {
            // 1. Lấy dữ liệu Dòng (Nhân sự & Chấm công)
            var dsChamCong = await _context.BangChamCongNhanViens.Where(c => c.BangChamCongId == BangChamCongId).ToListAsync();
            var dsHoSo = await _context.HoSoLuongs.ToListAsync();

            // 2. Lấy dữ liệu Cột (Các thành phần lương trong Mẫu đã chọn)
            var mauLuong = await _context.MauBangLuongs
                .Include(m => m.ChiTietMaus).ThenInclude(c => c.ThanhPhanLuong)
                .FirstOrDefaultAsync(m => m.Id == MauBangLuongId);

            var bangLuongMoi = new BangLuong { TenBangLuong = TenBangLuong, Thang = Thang, Nam = Nam, BangChamCongId = BangChamCongId, MauBangLuongId = MauBangLuongId, TrangThai = "Bản nháp", NgayTao = DateTime.Now };
            _context.BangLuongs.Add(bangLuongMoi);
            await _context.SaveChangesAsync();

            decimal tongQuy = 0;
            foreach (var cc in dsChamCong)
            {
                var hoSo = dsHoSo.FirstOrDefault(h => h.NhanSuId == cc.NhanSuId);
                if (hoSo == null) continue;

                var ct = new ChiTietBangLuong { BangLuongId = bangLuongMoi.Id, NhanSuId = cc.NhanSuId, LuongCoBan = hoSo.LuongCoBan, LuongDongBaoHiem = hoSo.LuongDongBaoHiem, CongHuongLuong = cc.TongCongHuongLuong };

                decimal tongThuNhap = 0, tongKhauTru = 0;
                var listKetQua = new List<KetQuaLuong>();

                // QUÉT QUA TỪNG CỘT TRONG MẪU LƯƠNG ĐỂ TÍNH TIỀN
                foreach (var cot in mauLuong.ChiTietMaus.OrderBy(c => c.ThuTu))
                {
                    var tp = cot.ThanhPhanLuong;

                    // Truyền các biến số thực tế của nhân viên này vào hàm tính công thức
                    decimal soTien = TinhCongThuc(tp.GiaTri, hoSo, cc);

                    listKetQua.Add(new KetQuaLuong { ThanhPhanLuongId = tp.Id, SoTien = soTien });

                    if (tp.TinhChat == "Thu nhập") tongThuNhap += soTien;
                    else if (tp.TinhChat == "Khấu trừ") tongKhauTru += soTien;
                }

                ct.TongThuNhap = tongThuNhap;
                ct.TongKhauTru = tongKhauTru;
                ct.ThucLinh = tongThuNhap - tongKhauTru;
                ct.KetQuaLuongs = listKetQua;

                tongQuy += ct.ThucLinh;
                _context.ChiTietBangLuongs.Add(ct);
            }

            bangLuongMoi.TongQuyLuong = tongQuy;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = bangLuongMoi.Id });
        }

        // HÀM DỊCH CÔNG THỨ
        private decimal TinhCongThuc(string? giaTri, HoSoLuong hs, BangChamCongNhanVien cc)
        {
            if (string.IsNullOrEmpty(giaTri)) return 0;
            if (!giaTri.StartsWith("=")) return Convert.ToDecimal(giaTri);

            var culture = System.Globalization.CultureInfo.InvariantCulture;

            // Tự động thay thế các chữ cái thành con số thực tế của nhân viên
            string bieuThuc = giaTri.Replace("=", "").ToUpper()
                .Replace("LUONG_CO_BAN/NGAY_CONG", hs.LuongCoBan.ToString("0.##", culture)) 
                .Replace("LUONG_CO_BAN", hs.LuongCoBan.ToString("0.##", culture))        
                .Replace("LCB", hs.LuongCoBan.ToString("0.##", culture))                   
                .Replace("LDBH", hs.LuongDongBaoHiem.ToString("0.##", culture))
                .Replace("SO_NPT", hs.SoNguoiPhuThuoc.ToString(culture))
                .Replace("CONG_THUC_TE", cc.TongCongHuongLuong.ToString(culture))
                .Replace("GIO_OT", cc.TongGioTangCa.ToString(culture))
                .Replace("PHUT_TRE", cc.TongPhutDiMuonVeSom.ToString(culture))
                .Replace("PC_CHUCVU", hs.PhuCapChucVu.ToString("0.##", culture))
                .Replace("PC_DILAI", hs.PhuCapDiLai.ToString("0.##", culture))
                .Replace("PC_AN", hs.PhuCapKhac.ToString("0.##", culture))
                .Replace("CONG_CHUAN", "26");

            try
            {
                return Convert.ToDecimal(new DataTable().Compute(bieuThuc, null));
            }
            catch
            {
                return 0;
            }
        }

        // 3. XEM CHI TIẾT
        public async Task<IActionResult> Detail(int id)
        {
            // Load Bảng lương + Mẫu Bảng Lương (để lấy Tiêu đề cột)
            var bangLuong = await _context.BangLuongs
                .Include(b => b.BangChamCong)
                .Include(b => b.MauBangLuong).ThenInclude(m => m.ChiTietMaus).ThenInclude(c => c.ThanhPhanLuong)
                .FirstOrDefaultAsync(b => b.Id == id);

            // Load Chi tiết + Kết quả động (để lấy Dữ liệu ô)
            var chiTiet = await _context.ChiTietBangLuongs
                .Include(c => c.NhanSu)
                .Include(c => c.KetQuaLuongs)
                .Where(c => c.BangLuongId == id)
                .ToListAsync();

            ViewBag.BangLuong = bangLuong;
            return View(chiTiet);
        }

        // 4. CẬP NHẬT LẠI BẢNG LƯƠNG
        [HttpPost]
        [Authorize(Policy = "Payroll.Manage")]
        public async Task<IActionResult> Recalculate(int id)
        {
            var bangLuong = await _context.BangLuongs
                .Include(b => b.MauBangLuong).ThenInclude(m => m.ChiTietMaus).ThenInclude(c => c.ThanhPhanLuong)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bangLuong == null) return NotFound();

            if (bangLuong.TrangThai == "Đã duyệt" || bangLuong.TrangThai == "Đã chi trả")
            {
                TempData["ErrorMessage"] = "Không thể cập nhật bảng lương đã được duyệt!";
                return RedirectToAction(nameof(Detail), new { id = id });
            }

            // Xóa chi tiết cũ
            var chiTietCu = await _context.ChiTietBangLuongs
                .Include(c => c.KetQuaLuongs)
                .Where(c => c.BangLuongId == id).ToListAsync();
            _context.ChiTietBangLuongs.RemoveRange(chiTietCu);
            await _context.SaveChangesAsync();

            // Kéo dữ liệu chạy lại
            var dsChamCong = await _context.BangChamCongNhanViens.Where(c => c.BangChamCongId == bangLuong.BangChamCongId).ToListAsync();
            var dsHoSo = await _context.HoSoLuongs.ToListAsync();

            decimal tongQuy = 0;
            foreach (var cc in dsChamCong)
            {
                var hoSo = dsHoSo.FirstOrDefault(h => h.NhanSuId == cc.NhanSuId);
                if (hoSo == null) continue;

                var ct = new ChiTietBangLuong { BangLuongId = bangLuong.Id, NhanSuId = cc.NhanSuId, LuongCoBan = hoSo.LuongCoBan, LuongDongBaoHiem = hoSo.LuongDongBaoHiem, CongHuongLuong = cc.TongCongHuongLuong };

                decimal tongThuNhap = 0, tongKhauTru = 0;
                var listKetQua = new List<KetQuaLuong>();

                foreach (var cot in bangLuong.MauBangLuong.ChiTietMaus.OrderBy(c => c.ThuTu))
                {
                    var tp = cot.ThanhPhanLuong;
                    decimal soTien = TinhCongThuc(tp.GiaTri, hoSo, cc);

                    listKetQua.Add(new KetQuaLuong { ThanhPhanLuongId = tp.Id, SoTien = soTien });

                    if (tp.TinhChat == "Thu nhập") tongThuNhap += soTien;
                    else if (tp.TinhChat == "Khấu trừ") tongKhauTru += soTien;
                }

                ct.TongThuNhap = tongThuNhap;
                ct.TongKhauTru = tongKhauTru;
                ct.ThucLinh = tongThuNhap - tongKhauTru;
                ct.KetQuaLuongs = listKetQua;

                tongQuy += ct.ThucLinh;
                _context.ChiTietBangLuongs.Add(ct);
            }

            bangLuong.TongQuyLuong = tongQuy;
            _context.BangLuongs.Update(bangLuong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã cập nhật và tính toán lại bảng lương thành công!";
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> SendPayslips(int id)
        {
            var bangLuong = await _context.BangLuongs.FindAsync(id);
            if (bangLuong == null) return NotFound();

            // Đổi trạng thái bảng lương tổng
            bangLuong.TrangThai = "Đã gửi phiếu lương";

            // Cập nhật trạng thái của từng dòng chi tiết thành "Chờ xác nhận"
            var chiTiet = await _context.ChiTietBangLuongs.Where(c => c.BangLuongId == id).ToListAsync();
            foreach (var item in chiTiet)
            {
                item.TrangThaiXacNhan = "Chờ xác nhận";
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã gửi phiếu lương điện tử đến tất cả nhân viên!";
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        // 5. XÓA BẢNG LƯƠNG
        [HttpPost]
        [Authorize(Policy = "Payroll.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var bangLuong = await _context.BangLuongs.FindAsync(id);
            if (bangLuong == null) return NotFound();

            // QUY TẮC: Đã duyệt thì KHÔNG được xóa. (Nháp và Đã gửi nhân viên thì được xóa)
            if (bangLuong.TrangThai == "Đã duyệt" || bangLuong.TrangThai == "Đã chi trả")
            {
                TempData["ErrorMessage"] = "Không thể xóa bảng lương đã được duyệt hoặc chi trả!";
                return RedirectToAction(nameof(Index));
            }

            var chiTiet = await _context.ChiTietBangLuongs
                .Include(c => c.KetQuaLuongs)
                .Where(c => c.BangLuongId == id)
                .ToListAsync();

            _context.ChiTietBangLuongs.RemoveRange(chiTiet);
            _context.BangLuongs.Remove(bangLuong);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa bảng lương thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 7. DUYỆT BẢNG LƯƠNG
        [HttpPost]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var bangLuong = await _context.BangLuongs.FindAsync(id);
            if (bangLuong == null) return NotFound();

            // QUY TẮC 1: Chưa gửi nhân viên (Bản nháp) -> KHÔNG ĐƯỢC DUYỆT
            if (bangLuong.TrangThai == "Bản nháp")
            {
                TempData["ErrorMessage"] = "Bạn phải Gửi phiếu lương cho nhân viên kiểm tra trước khi Duyệt!";
                return RedirectToAction(nameof(Detail), new { id = id });
            }

            // QUY TẮC 2: Đã duyệt rồi -> KHÔNG ĐƯỢC DUYỆT LẠI
            if (bangLuong.TrangThai == "Đã duyệt" || bangLuong.TrangThai == "Đã chi trả")
            {
                TempData["ErrorMessage"] = "Bảng lương này đã được duyệt rồi!";
                return RedirectToAction(nameof(Detail), new { id = id });
            }

            // QUY TẮC 3: Đã gửi nhân viên -> ĐƯỢC DUYỆT
            bangLuong.TrangThai = "Đã duyệt";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã duyệt bảng lương thành công. Dữ liệu đã được khóa!";
            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
