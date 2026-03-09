using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == currentUserId);
            if (nhanSu == null) return NotFound("Không tìm thấy hồ sơ nhân sự.");

            int currentYear = DateTime.Now.Year;

            // 1. Lấy thông tin Quỹ phép của nhân viên trong năm nay
            var quyPhep = await _context.QuyPhepNhanViens
                .FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == currentYear);

            // 2. TÍNH TOÁN CÁC CON SỐ CHO 4 THẺ (CARD)
            // Lấy các tham số (Bao gồm cả Tồn, Thâm niên, Thưởng vừa tạo ở các bước trước)
            double tongPhepNam = quyPhep?.TongPhepNam ?? 0;
            double phepTon = quyPhep?.PhepTonNamTruoc ?? 0;
            double phepThamNien = quyPhep?.PhepThamNien ?? 0;
            double phepThuong = quyPhep?.PhepThuong ?? 0;
            double phepDaDung = quyPhep?.SoPhepDaDung ?? 0;

            // TÍNH THẺ 1 & 3: Tổng khả dụng cả năm & Còn lại cả năm
            double tongPhepCaNam = tongPhepNam + phepTon + phepThamNien + phepThuong;
            double phepConLaiCaNam = tongPhepCaNam - phepDaDung;

            // TÍNH THẺ 4: Còn lại đến tháng hiện tại (Sửa lỗi hiển thị ở đây)
            int thangHienTai = DateTime.Now.Month;

            // a. Tích lũy đến hiện tại (VD: Tháng 3 được 3 ngày)
            double phepTichLuyThang = Math.Round((tongPhepNam / 12.0) * thangHienTai, 1);

            // b. Cộng thêm Tồn, Thâm niên, Thưởng (Vì các phép này được xài ngay từ đầu năm)
            double tongKhaDungDenHienTai = phepTichLuyThang + phepTon + phepThamNien + phepThuong;

            // c. CHÍNH XÁC: Phải trừ đi số phép đã dùng!
            double phepConLaiDenThangHienTai = tongKhaDungDenHienTai - phepDaDung;

            // Đảm bảo UI không bị hiện số âm nếu có lỗi dữ liệu
            if (phepConLaiDenThangHienTai < 0) phepConLaiDenThangHienTai = 0;

            // 3. Lấy danh sách toàn bộ Đơn xin nghỉ
            var danhSachDon = await _context.DonXinNghis
                .Include(d => d.LoaiNghiPhep)
                .Include(d => d.NhanSu)
                .Include(d => d.NguoiDuyet) // Bổ sung NguoiDuyet để hiển thị tên sếp
                .Where(d => d.NhanSuId == nhanSu.Id)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            // 4. Tính toán số ngày nghỉ không lương (Thẻ thứ 5)
            double nghiKhongLuong = danhSachDon
                .Where(d => d.TrangThai == "Đã duyệt" && d.LoaiNghiPhep != null && d.LoaiNghiPhep.CoHuongLuong == false)
                .Sum(d => d.SoNgayNghi);

            // 5. Đẩy dữ liệu ra ViewBag
            ViewBag.TongPhepNam = tongPhepCaNam;
            ViewBag.PhepDaDung = phepDaDung;
            ViewBag.PhepConLai = phepConLaiCaNam;
            ViewBag.PhepTheoThang = phepConLaiDenThangHienTai; 
            ViewBag.NghiKhongLuong = nghiKhongLuong;

            return View(danhSachDon);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nhanSu == null) return NotFound("Lỗi: Không tìm thấy hồ sơ nhân sự!");

            // 1. Lấy danh sách Loại nghỉ phép truyền ra View (Dropdown)
            ViewBag.LoaiNghiPheps = new SelectList(await _context.LoaiNghiPheps.Where(x => x.KichHoat).ToListAsync(), "Id", "TenLoai");

            // 2. Lấy thông tin Quỹ phép của nhân viên để hiển thị bên cột phải
            var quyPhep = await _context.QuyPhepNhanViens.FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == DateTime.Now.Year);

            if (quyPhep != null)
            {
                int thangHienTai = DateTime.Now.Month;

                // Tính số phép tiêu chuẩn được cấp tính đến tháng hiện tại
                double phepTichLuy = Math.Round((quyPhep.TongPhepNam / 12.0) * thangHienTai, 1);

                // Tổng số phép NHÂN VIÊN ĐƯỢC PHÉP XÀI = Phép tích lũy + Tồn năm trước + Thâm niên + Thưởng
                double tongPhepHienTai = phepTichLuy + quyPhep.PhepTonNamTruoc + quyPhep.PhepThamNien + quyPhep.PhepThuong;

                ViewBag.TongPhep = tongPhepHienTai;
                ViewBag.DaDung = quyPhep.SoPhepDaDung;
                ViewBag.ConLai = tongPhepHienTai - quyPhep.SoPhepDaDung; // Toán học khớp 100%
            }
            else
            {
                ViewBag.TongPhep = 0;
                ViewBag.DaDung = 0;
                ViewBag.ConLai = 0;
            }

            // 3. Gán giá trị mặc định cho form
            var model = new DonXinNghi
            {
                TuNgay = DateTime.Today.AddHours(8),
                DenNgay = DateTime.Today.AddHours(17),
                SoNgayNghi = 1
            };

            return View(model);
        }

        // --- 2. HÀM XỬ LÝ LƯU ĐƠN VÀ FILE ĐÍNH KÈM ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonXinNghi model, IFormFile? fileUpload)
        {
            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);

            // ==========================================
            // 1. XÓA VALIDATE CHO CÁC TRƯỜNG KHÔNG CÓ TRÊN FORM
            // ==========================================
            ModelState.Remove("NhanSu");
            ModelState.Remove("LoaiNghiPhep");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");
            ModelState.Remove("FileDinhKem");
            ModelState.Remove("fileUpload"); // <--- Thêm dòng này để không bắt buộc upload file

            if (ModelState.IsValid)
            {
                // ========================================================
                // 2. KIỂM TRA QUỸ PHÉP TÍCH LŨY (CÁCH 2) TRƯỚC KHI CHO LƯU
                // ========================================================
                var loaiNghi = await _context.LoaiNghiPheps.FindAsync(model.LoaiNghiPhepId);

                // Chỉ kiểm tra nếu loại nghỉ này CÓ TRỪ VÀO QUỸ PHÉP (VD: Nghỉ phép năm)
                if (loaiNghi != null && loaiNghi.TruVaoQuyPhep == true)
                {
                    var quyPhep = await _context.QuyPhepNhanViens
                        .FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == DateTime.Now.Year);

                    if (quyPhep != null)
                    {
                        int thangHienTai = DateTime.Now.Month; // Lấy tháng hiện tại (VD: Tháng 2)

                        // Tính số phép được cấp tính đến tháng hiện tại
                        // Ép kiểu 12.0 để chia ra số thập phân chính xác
                        double phepTichLuy = Math.Round((quyPhep.TongPhepNam / 12.0) * thangHienTai, 1);

                        // Nếu bạn có làm các cột phép tồn, thâm niên, thưởng thì bỏ comment dòng dưới:
                        phepTichLuy += quyPhep.PhepTonNamTruoc + quyPhep.PhepThamNien + quyPhep.PhepThuong;

                        // Tính số phép thực sự còn lại để dùng
                        double phepThucTeConLai = phepTichLuy - quyPhep.SoPhepDaDung;

                        // CHẶN LẠI NẾU XIN VƯỢT QUÁ SỐ PHÉP ĐANG CÓ
                        if (model.SoNgayNghi > phepThucTeConLai)
                        {
                            // Bắn lỗi ra màn hình cho nhân viên biết
                            ModelState.AddModelError(string.Empty,
                                $"Tính đến tháng {thangHienTai}, bạn chỉ tích lũy được {phepThucTeConLai} ngày phép. Bạn đang xin nghỉ {model.SoNgayNghi} ngày. Vui lòng giảm số ngày hoặc đổi sang Nghỉ không lương.");

                            // Phục hồi lại giao diện
                            ViewBag.LoaiNghiPheps = new SelectList(await _context.LoaiNghiPheps.Where(x => x.KichHoat).ToListAsync(), "Id", "TenLoai", model.LoaiNghiPhepId);
                            ViewBag.TongPhep = quyPhep.TongPhepNam;
                            ViewBag.DaDung = quyPhep.SoPhepDaDung;
                            ViewBag.ConLai = quyPhep.SoPhepConLai;

                            // Dừng lại ở đây, trả về View hiện lỗi, KHÔNG lưu Database
                            return View(model);
                        }
                    }
                }
                // ================== KẾT THÚC KIỂM TRA ===================

                // 3. NẾU HỢP LỆ, GÁN DỮ LIỆU VÀ LƯU
                model.NhanSuId = nhanSu.Id;
                model.TrangThai = "Chờ duyệt";
                model.NgayTao = DateTime.Now;
                model.NguoiDuyetId = null;

                // XỬ LÝ UPLOAD FILE
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "donxinnghi");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }
                    model.FileDinhKem = "/uploads/donxinnghi/" + uniqueFileName;
                }

                _context.DonXinNghis.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã gửi đơn xin nghỉ thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Load lại Dropdown và Quỹ phép nếu form bị lỗi ngầm định
            ViewBag.LoaiNghiPheps = new SelectList(await _context.LoaiNghiPheps.Where(x => x.KichHoat).ToListAsync(), "Id", "TenLoai", model.LoaiNghiPhepId);

            var qp = await _context.QuyPhepNhanViens.FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == DateTime.Now.Year);
            ViewBag.TongPhep = qp?.TongPhepNam ?? 0;
            ViewBag.DaDung = qp?.SoPhepDaDung ?? 0;
            ViewBag.ConLai = qp?.SoPhepConLai ?? 0;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nhanSu == null) return NotFound("Lỗi: Không tìm thấy hồ sơ nhân sự!");

            // Tìm đơn trong Database (Chỉ lấy đơn của chính người đang đăng nhập)
            var don = await _context.DonXinNghis.FirstOrDefaultAsync(d => d.Id == id && d.NhanSuId == nhanSu.Id);
            if (don == null) return NotFound("Không tìm thấy đơn hoặc bạn không có quyền sửa đơn này!");

            // KIỂM TRA QUY ĐỊNH: Nếu đã duyệt thì không cho sửa
            if (don.TrangThai == "Đã duyệt")
            {
                TempData["ErrorMessage"] = "Đơn này đã được duyệt, bạn không thể chỉnh sửa!";
                return RedirectToAction(nameof(Index));
            }

            // Lấy dữ liệu cho Dropdown
            ViewBag.LoaiNghiPheps = new SelectList(await _context.LoaiNghiPheps.Where(x => x.KichHoat).ToListAsync(), "Id", "TenLoai", don.LoaiNghiPhepId);

            var quyPhep = await _context.QuyPhepNhanViens.FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == DateTime.Now.Year);

            if (quyPhep != null)
            {
                int thangHienTai = DateTime.Now.Month;

                // Tính số phép tiêu chuẩn được cấp tính đến tháng hiện tại
                double phepTichLuy = Math.Round((quyPhep.TongPhepNam / 12.0) * thangHienTai, 1);

                // Tổng số phép NHÂN VIÊN ĐƯỢC PHÉP XÀI = Phép tích lũy + Tồn năm trước + Thâm niên + Thưởng
                double tongPhepHienTai = phepTichLuy + quyPhep.PhepTonNamTruoc + quyPhep.PhepThamNien + quyPhep.PhepThuong;

                ViewBag.TongPhep = tongPhepHienTai;
                ViewBag.DaDung = quyPhep.SoPhepDaDung;
                ViewBag.ConLai = tongPhepHienTai - quyPhep.SoPhepDaDung;
            }
            else
            {
                ViewBag.TongPhep = 0;
                ViewBag.DaDung = 0;
                ViewBag.ConLai = 0;
            }

            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonXinNghi model, IFormFile? fileUpload)
        {
            if (id != model.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var nhanSu = await _context.NhanSus.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nhanSu == null) return NotFound();

            // 1. Lấy lá đơn GỐC từ Database ra để kiểm tra
            var donGoc = await _context.DonXinNghis.FindAsync(id);
            if (donGoc == null || donGoc.NhanSuId != nhanSu.Id) return NotFound();

            // CHẶN BẢO MẬT: Đề phòng user lách luật gửi request ảo
            if (donGoc.TrangThai == "Đã duyệt")
            {
                TempData["ErrorMessage"] = "Đơn đã duyệt không thể lưu thay đổi!";
                return RedirectToAction(nameof(Index));
            }

            // Gỡ bỏ validate các biến ngầm định
            ModelState.Remove("NhanSu");
            ModelState.Remove("LoaiNghiPhep");
            ModelState.Remove("NguoiDuyet");
            ModelState.Remove("TrangThai");
            ModelState.Remove("FileDinhKem");
            ModelState.Remove("fileUpload");

            if (ModelState.IsValid)
            {
                // ========================================================
                // 2. KIỂM TRA QUỸ PHÉP TÍCH LŨY TRƯỚC KHI CHO LƯU SỬA
                // ========================================================
                var loaiNghi = await _context.LoaiNghiPheps.FindAsync(model.LoaiNghiPhepId);

                if (loaiNghi != null && loaiNghi.TruVaoQuyPhep == true)
                {
                    var quyPhep = await _context.QuyPhepNhanViens
                        .FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == DateTime.Now.Year);

                    if (quyPhep != null)
                    {
                        int thangHienTai = DateTime.Now.Month;
                        double phepTichLuy = Math.Round((quyPhep.TongPhepNam / 12.0) * thangHienTai, 1);
                        double tongPhepHienTai = phepTichLuy + quyPhep.PhepTonNamTruoc + quyPhep.PhepThamNien + quyPhep.PhepThuong;
                        double phepThucTeConLai = tongPhepHienTai - quyPhep.SoPhepDaDung;

                        // NẾU SỐ NGÀY MỚI SỬA VƯỢT QUÁ SỐ PHÉP -> CHẶN
                        if (model.SoNgayNghi > phepThucTeConLai)
                        {
                            ModelState.AddModelError(string.Empty,
                                $"Tính đến tháng {thangHienTai}, bạn chỉ tích lũy được {phepThucTeConLai} ngày phép. Bạn đang xin nghỉ {model.SoNgayNghi} ngày. Vui lòng giảm số ngày hoặc đổi loại nghỉ.");

                            // Phục hồi lại dữ liệu cho Form để hiển thị lỗi
                            ViewBag.LoaiNghiPheps = new SelectList(await _context.LoaiNghiPheps.Where(x => x.KichHoat).ToListAsync(), "Id", "TenLoai", model.LoaiNghiPhepId);
                            ViewBag.TongPhep = tongPhepHienTai;
                            ViewBag.DaDung = quyPhep.SoPhepDaDung;
                            ViewBag.ConLai = phepThucTeConLai;

                            // Giữ lại đường dẫn file đính kèm cũ để View hiển thị
                            model.FileDinhKem = donGoc.FileDinhKem;
                            return View(model);
                        }
                    }
                }
                // ================== KẾT THÚC KIỂM TRA ===================

                // 3. NẾU HỢP LỆ, CẬP NHẬT DỮ LIỆU
                donGoc.TuNgay = model.TuNgay;
                donGoc.DenNgay = model.DenNgay;
                donGoc.SoNgayNghi = model.SoNgayNghi;
                donGoc.LoaiNghiPhepId = model.LoaiNghiPhepId;
                donGoc.LyDo = model.LyDo;
                donGoc.TrangThai = "Chờ duyệt"; // Sửa đơn thì sẽ quay về Chờ duyệt

                // Xử lý File đính kèm mới (nếu có)
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "donxinnghi");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }
                    donGoc.FileDinhKem = "/uploads/donxinnghi/" + uniqueFileName;
                }

                _context.Update(donGoc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật đơn xin nghỉ thành công!";
                return RedirectToAction(nameof(Index));
            }

            // 4. CHẠY KHI FORM BỊ LỖI NGẦM ĐỊNH (ModelState.IsValid = false)
            ViewBag.LoaiNghiPheps = new SelectList(await _context.LoaiNghiPheps.Where(x => x.KichHoat).ToListAsync(), "Id", "TenLoai", model.LoaiNghiPhepId);

            var qp = await _context.QuyPhepNhanViens.FirstOrDefaultAsync(q => q.NhanSuId == nhanSu.Id && q.Nam == DateTime.Now.Year);
            if (qp != null)
            {
                int thangHienTai = DateTime.Now.Month;
                double phepTichLuy = Math.Round((qp.TongPhepNam / 12.0) * thangHienTai, 1);
                double tongPhepHienTai = phepTichLuy + qp.PhepTonNamTruoc + qp.PhepThamNien + qp.PhepThuong;

                ViewBag.TongPhep = tongPhepHienTai;
                ViewBag.DaDung = qp.SoPhepDaDung;
                ViewBag.ConLai = tongPhepHienTai - qp.SoPhepDaDung;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var don = await _context.DonXinNghis.FindAsync(id);
            if (don == null) return Json(new { success = false, message = "Không tìm thấy đơn!" });

            if (don.TrangThai == "Đã duyệt")
                return Json(new { success = false, message = "Không thể xóa đơn đã được duyệt!" });

            _context.DonXinNghis.Remove(don);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa đơn thành công!" });
        }
    }
}
