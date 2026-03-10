using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TRANG DANH SÁCH PHIẾU CHI
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.PhieuChiLuongs
                .Include(p => p.BangLuong)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();

            // Lấy danh sách bảng lương Đã duyệt hoặc Đang chi trả dở dang để chọn
            var bangLuongs = await _context.BangLuongs
                .Where(b => b.TrangThai == "Đã duyệt" || b.TrangThai == "Đang chi trả")
                .Select(b => new {
                    b.Id,
                    TenHienThi = $"{b.TenBangLuong} (Tổng quỹ: {b.TongQuyLuong.ToString("N0")} đ)"
                })
                .ToListAsync();

            ViewBag.BangLuongList = new SelectList(bangLuongs, "Id", "TenHienThi");

            // Thống kê thẻ trên cùng
            ViewBag.TongDaChi = danhSach.Where(p => p.TrangThai == "Hoàn thành").Sum(p => p.SoTienChi);
            ViewBag.SoPhieu = danhSach.Count;

            return View(danhSach);
        }

        // 2. TẠO PHIẾU CHI MỚI (CHỈ TẠO VỎ VÀ KÉO DANH SÁCH NHÂN VIÊN)
        [HttpPost]
        public async Task<IActionResult> Create(PhieuChiLuong model)
        {
            var bangLuong = await _context.BangLuongs.FindAsync(model.BangLuongId);
            if (bangLuong == null) return NotFound();

            // Khởi tạo vỏ phiếu chi
            model.MaPhieuChi = "PC-" + DateTime.Now.ToString("MMyy") + "-" + new Random().Next(100, 999);
            model.NgayTao = DateTime.Now;
            model.TrangThai = "Bản nháp";
            model.SoTienChi = 0;
            _context.PhieuChiLuongs.Add(model);
            await _context.SaveChangesAsync(); // Lưu để lấy ID Phiếu Chi

            // Kéo danh sách nhân viên của bảng lương này vào phiếu chi
            var dsNhanVien = await _context.ChiTietBangLuongs.Where(c => c.BangLuongId == model.BangLuongId).ToListAsync();
            decimal tongTienPhieu = 0;

            foreach (var nv in dsNhanVien)
            {
                // Tính xem đợt trước đã trả cho người này bao nhiêu
                var daChi = await _context.ChiTietPhieuChiLuongs
                    .Where(p => p.ChiTietBangLuongId == nv.Id && p.PhieuChiLuongId != model.Id && p.PhieuChiLuong.TrangThai == "Hoàn thành")
                    .SumAsync(p => p.SoTienChi);

                var conLai = nv.ThucLinh - daChi;

                // Chỉ thêm những người còn nợ tiền vào phiếu chi đợt này
                if (conLai > 0)
                {
                    var chiTietPhieu = new ChiTietPhieuChiLuong
                    {
                        PhieuChiLuongId = model.Id,
                        ChiTietBangLuongId = nv.Id,
                        SoTienChi = conLai // Gợi ý mặc định là trả nốt số tiền còn nợ
                    };
                    _context.ChiTietPhieuChiLuongs.Add(chiTietPhieu);
                    tongTienPhieu += conLai;
                }
            }

            model.SoTienChi = tongTienPhieu;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã khởi tạo phiếu chi. Vui lòng kiểm tra và điều chỉnh số tiền cho từng nhân viên!";
            // Chuyển thẳng sang trang Detail để sửa tiền
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }

        // 3. TRANG CHI TIẾT (BẢNG NHẬP TIỀN CỦA TỪNG NGƯỜI)
        public async Task<IActionResult> Detail(int id)
        {
            var phieu = await _context.PhieuChiLuongs
                .Include(p => p.BangLuong)
                .Include(p => p.ChiTietPhieuChiLuongs)
                    .ThenInclude(c => c.ChiTietBangLuong)
                        .ThenInclude(ct => ct.NhanSu)
                            .ThenInclude(ns => ns.PhongBan)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (phieu == null) return NotFound();

            // Tính số tiền đã chi ở CÁC PHIẾU TRƯỚC (để hiển thị lên giao diện)
            var dictDaChiTruocDo = new Dictionary<int, decimal>();
            foreach (var ct in phieu.ChiTietPhieuChiLuongs)
            {
                var daChiTruoc = await _context.ChiTietPhieuChiLuongs
                    .Where(x => x.ChiTietBangLuongId == ct.ChiTietBangLuongId && x.PhieuChiLuongId != id && x.PhieuChiLuong.TrangThai == "Hoàn thành")
                    .SumAsync(x => x.SoTienChi);
                dictDaChiTruocDo[ct.ChiTietBangLuongId] = daChiTruoc;
            }
            ViewBag.DaChiTruocDo = dictDaChiTruocDo;

            return View(phieu);
        }

        // 4. LƯU SỐ TIỀN & CHỐT PHIẾU CHI
        [HttpPost]
        public async Task<IActionResult> UpdateChiTiet(int PhieuChiId, int[] ChiTietPhieuIds, decimal[] SoTienChis)
        {
            var phieu = await _context.PhieuChiLuongs.FindAsync(PhieuChiId);
            if (phieu == null) return NotFound();

            decimal tongTienMoi = 0;
            // Cập nhật lại số tiền do kế toán vừa sửa trên giao diện
            for (int i = 0; i < ChiTietPhieuIds.Length; i++)
            {
                var ct = await _context.ChiTietPhieuChiLuongs.FindAsync(ChiTietPhieuIds[i]);
                if (ct != null)
                {
                    ct.SoTienChi = SoTienChis[i];
                    tongTienMoi += SoTienChis[i];
                }
            }

            phieu.SoTienChi = tongTienMoi;
            phieu.TrangThai = "Hoàn thành";
            await _context.SaveChangesAsync();

            // Cập nhật trạng thái của Bảng Lương (Đang chi trả hay Đã chi trả xong)
            var bangLuong = await _context.BangLuongs.FindAsync(phieu.BangLuongId);
            var tongDaChiCuaBangLuong = await _context.PhieuChiLuongs
                .Where(p => p.BangLuongId == bangLuong.Id && p.TrangThai == "Hoàn thành")
                .SumAsync(p => p.SoTienChi);

            if (tongDaChiCuaBangLuong >= bangLuong.TongQuyLuong)
                bangLuong.TrangThai = "Đã chi trả";
            else
                bangLuong.TrangThai = "Đang chi trả";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã lưu số liệu và chốt phiếu chi thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 5. XÓA PHIẾU CHI NHÁP (Tùy chọn)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var phieu = await _context.PhieuChiLuongs.FindAsync(id);
            if (phieu != null && phieu.TrangThai == "Bản nháp")
            {
                var chiTiet = await _context.ChiTietPhieuChiLuongs.Where(c => c.PhieuChiLuongId == id).ToListAsync();
                _context.ChiTietPhieuChiLuongs.RemoveRange(chiTiet);
                _context.PhieuChiLuongs.Remove(phieu);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa phiếu chi nháp thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
