using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;

namespace SDHRM.Areas.Timesheet.Controllers
{
    [Area("Timesheet")]
    [Authorize(Policy = "Reports.View")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region HÀM LÕI DÙNG CHUNG (CORE HELPERS)

        // 1. HÀM LẤY DỮ LIỆU GỐC CHUNG: Dùng cho mọi báo cáo liên quan đến chấm công ngày
        private IQueryable<ChiTietChamCongNgay> GetBaseAttendanceQuery(DateTime startDate, DateTime endDate, string phongBan)
        {
            var query = _context.ChiTietChamCongNgays
                .Include(c => c.BangChamCongNhanVien)
                    .ThenInclude(b => b.NhanSu)
                        .ThenInclude(n => n.PhongBan)
                .Include(c => c.BangChamCongNhanVien)
                    .ThenInclude(b => b.NhanSu)
                        .ThenInclude(n => n.ViTriCongViec)
                .Where(c => c.Ngay.Date >= startDate.Date && c.Ngay.Date <= endDate.Date)
                .AsQueryable();

            if (!string.IsNullOrEmpty(phongBan))
                query = query.Where(c => c.BangChamCongNhanVien.NhanSu.PhongBan.TenPhongBan == phongBan);

            return query;
        }

        // 2. HÀM XUẤT EXCEL CHUNG: Dùng cho mọi loại class (T), mọi loại báo cáo
        private IActionResult ExportExcelChung<T>(
            List<T> data,
            string sheetName,
            string fileName,
            string[] headers,
            Action<IXLWorksheet, int, T, int> fillDataRow)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                // Tạo Tiêu đề
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }
                worksheet.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
                worksheet.Range(1, 1, 1, headers.Length).Style.Fill.BackgroundColor = XLColor.LightGray;

                // Đổ Dữ liệu
                int row = 2;
                int stt = 1;
                foreach (var item in data)
                {
                    fillDataRow(worksheet, row, item, stt); // Gọi hàm ủy quyền
                    row++;
                    stt++;
                }

                worksheet.Columns().AdjustToContents();

                // Trả về file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion


        #region BÁO CÁO 1: CHI TIẾT ĐI MUỘN VỀ SỚM NGHỈ
        private async Task<List<SDHRM.Models.ViewModels.ChiTietDiMuonVeSomVM>> GetDataChiTiet(DateTime startDate, DateTime endDate, string xemTheo, string phongBan)
        {
            var query = GetBaseAttendanceQuery(startDate, endDate, phongBan); // GỌI HÀM LÕI
            var dataList = await query.ToListAsync();
            var reportData = new List<SDHRM.Models.ViewModels.ChiTietDiMuonVeSomVM>();

            foreach (var c in dataList)
            {
                var nv = c.BangChamCongNhanVien.NhanSu;
                var tenPb = nv.PhongBan?.TenPhongBan ?? "Chưa cập nhật";
                var maNv = nv.Id;

                if ((xemTheo == "TatCa" || xemTheo == "DiMuon") && c.PhutDiMuon > 0)
                    reportData.Add(new SDHRM.Models.ViewModels.ChiTietDiMuonVeSomVM { MaNV = maNv, HoTen = nv.HoTen, PhongBan = tenPb, Ngay = c.Ngay, Loai = "Đi muộn", SoPhut = c.PhutDiMuon, GhiChu = $"{c.PhutDiMuon} phút" });
                if ((xemTheo == "TatCa" || xemTheo == "VeSom") && c.PhutVeSom > 0)
                    reportData.Add(new SDHRM.Models.ViewModels.ChiTietDiMuonVeSomVM { MaNV = maNv, HoTen = nv.HoTen, PhongBan = tenPb, Ngay = c.Ngay, Loai = "Về sớm", SoPhut = c.PhutVeSom, GhiChu = $"{c.PhutVeSom} phút" });
                if ((xemTheo == "TatCa" || xemTheo == "Nghi") && new[] { "P", "V", "KL", "O" }.Contains(c.KyHieuChamCong))
                    reportData.Add(new SDHRM.Models.ViewModels.ChiTietDiMuonVeSomVM { MaNV = maNv, HoTen = nv.HoTen, PhongBan = tenPb, Ngay = c.Ngay, Loai = "Nghỉ", SoPhut = 0, GhiChu = c.LyDo ?? $"Nghỉ ({c.KyHieuChamCong})" });
            }

            return reportData.OrderByDescending(x => x.Ngay).ThenBy(x => x.HoTen).ToList();
        }

        public async Task<IActionResult> LateEarlyListReport(DateTime? tuNgay, DateTime? denNgay, string xemTheo = "TatCa", string phongBan = "")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date; DateTime endDate = denNgay ?? DateTime.Now.Date;
            ViewBag.DanhSachPhongBan = await _context.PhongBans.Select(p => p.TenPhongBan).Distinct().ToListAsync();
            ViewBag.PhongBanDaChon = phongBan; ViewBag.TuNgay = startDate; ViewBag.DenNgay = endDate; ViewBag.XemTheo = xemTheo;

            var reportData = await GetDataChiTiet(startDate, endDate, xemTheo, phongBan);
            return View(reportData);
        }

        public async Task<IActionResult> ExportLateEarlyListReport(DateTime? tuNgay, DateTime? denNgay, string xemTheo = "TatCa", string phongBan = "")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date; DateTime endDate = denNgay ?? DateTime.Now.Date;
            var reportData = await GetDataChiTiet(startDate, endDate, xemTheo, phongBan);

            // GỌI HÀM XUẤT EXCEL CHUNG CHỈ VỚI 1 DÒNG CODE
            return ExportExcelChung(
                data: reportData,
                sheetName: "Chi tiết Đi muộn Về sớm",
                fileName: $"ChiTiet_DiMuonVeSom_{DateTime.Now:ddMMyyyy}.xlsx",
                headers: new[] { "STT", "Mã NV", "Tên nhân viên", "Phòng ban", "Ngày", "Ca", "Loại", "Số phút / Ghi chú" },
                fillDataRow: (ws, row, item, stt) => // Định nghĩa cách nhét dữ liệu vào cột
                {
                    ws.Cell(row, 1).Value = stt;
                    ws.Cell(row, 2).Value = item.MaNV;
                    ws.Cell(row, 3).Value = item.HoTen;
                    ws.Cell(row, 4).Value = item.PhongBan;
                    ws.Cell(row, 5).Value = item.Ngay.ToString("dd/MM/yyyy");
                    ws.Cell(row, 7).Value = item.Loai;
                    ws.Cell(row, 8).Value = item.GhiChu;
                }
            );
        }

        #endregion


        #region BÁO CÁO 2: TỔNG HỢP ĐI MUỘN VỀ SỚM
        private async Task<List<SDHRM.Models.ViewModels.BaoCaoDiMuonVeSomVM>> GetDataTongHop(DateTime startDate, DateTime endDate, string thongKeTheo, string phongBan)
        {
            var query = GetBaseAttendanceQuery(startDate, endDate, phongBan); // GỌI HÀM LÕI

            if (thongKeTheo == "DiMuon") query = query.Where(c => c.PhutDiMuon > 0);
            else if (thongKeTheo == "VeSom") query = query.Where(c => c.PhutVeSom > 0);
            else query = query.Where(c => c.PhutDiMuon > 0 || c.PhutVeSom > 0);

            var dataList = await query.ToListAsync();
            return dataList.GroupBy(c => new { c.BangChamCongNhanVien.NhanSu.Id, c.BangChamCongNhanVien.NhanSu.HoTen, TenPhongBan = c.BangChamCongNhanVien.NhanSu.PhongBan?.TenPhongBan, ViTriCongViec = c.BangChamCongNhanVien.NhanSu.ViTriCongViec?.TenViTri })
                .Select(g => new SDHRM.Models.ViewModels.BaoCaoDiMuonVeSomVM
                {
                    NhanSuId = g.Key.Id,
                    MaNV =  g.Key.Id,
                    HoTen = g.Key.HoTen,
                    ViTriCongViec = g.Key.ViTriCongViec ?? "Chưa cập nhật",
                    PhongBan = g.Key.TenPhongBan ?? "Chưa cập nhật",
                    SoLan = g.Count(),
                    TongSoPhut = g.Sum(c => thongKeTheo == "DiMuon" ? c.PhutDiMuon : thongKeTheo == "VeSom" ? c.PhutVeSom : c.PhutDiMuon + c.PhutVeSom)
                })
                .OrderByDescending(x => x.TongSoPhut).ToList();
        }

        public async Task<IActionResult> LateEarlyReport(DateTime? tuNgay, DateTime? denNgay, string thongKeTheo = "TatCa", string phongBan = "")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date; DateTime endDate = denNgay ?? DateTime.Now.Date;
            ViewBag.DanhSachPhongBan = await _context.PhongBans.Select(p => p.TenPhongBan).Distinct().ToListAsync();
            ViewBag.PhongBanDaChon = phongBan; ViewBag.TuNgay = startDate; ViewBag.DenNgay = endDate; ViewBag.ThongKeTheo = thongKeTheo;

            var reportData = await GetDataTongHop(startDate, endDate, thongKeTheo, phongBan);
            return View(reportData);
        }

        public async Task<IActionResult> ExportLateEarlyReport(DateTime? tuNgay, DateTime? denNgay, string thongKeTheo = "TatCa", string phongBan = "")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date; DateTime endDate = denNgay ?? DateTime.Now.Date;
            var reportData = await GetDataTongHop(startDate, endDate, thongKeTheo, phongBan);

            // GỌI HÀM XUẤT EXCEL CHUNG LẦN NỮA
            return ExportExcelChung(
                data: reportData,
                sheetName: "Tổng hợp Đi muộn Về sớm",
                fileName: $"TongHop_DiMuonVeSom_{startDate:ddMMyyyy}_{endDate:ddMMyyyy}.xlsx",
                headers: new[] { "STT", "Mã nhân viên", "Tên nhân viên", "Phòng ban", "Số lần", "Số phút" },
                fillDataRow: (ws, row, item, stt) => // Khai báo các cột cho báo cáo tổng hợp
                {
                    ws.Cell(row, 1).Value = stt;
                    ws.Cell(row, 2).Value = item.MaNV;
                    ws.Cell(row, 3).Value = item.HoTen;
                    ws.Cell(row, 4).Value = item.PhongBan;
                    ws.Cell(row, 5).Value = item.SoLan;
                    ws.Cell(row, 6).Value = item.TongSoPhut;
                }
            );
        }

        #endregion

        #region BÁO CÁO 3: TÌNH HÌNH NGHỈ PHÉP NĂM
        private async Task<List<SDHRM.Models.ViewModels.BaoCaoNghiPhepVM>> GetDataNghiPhep(int nam, string phongBan)
        {
            // Lấy danh sách nhân sự làm gốc
            var query = _context.NhanSus.Include(n => n.PhongBan).AsQueryable();

            if (!string.IsNullOrEmpty(phongBan))
                query = query.Where(n => n.PhongBan.TenPhongBan == phongBan);

            var nhanSus = await query.ToListAsync();


            var donNghis = await _context.DonXinNghis
                .Where(d => d.TrangThai == "Đã duyệt" && d.TuNgay.Year == nam && d.LoaiNghiPhep.Id == 1)                
                .ToListAsync();

            var reportData = nhanSus.Select(nv => new SDHRM.Models.ViewModels.BaoCaoNghiPhepVM
            {
                MaNV = nv.Id,
                HoTen = nv.HoTen,
                PhongBan = nv.PhongBan?.TenPhongBan ?? "Chưa cập nhật",

                PhepNamTruoc = 0,
                PhepTrongNam = 12,
                PhepThamNien = 0,

                DaNghi = donNghis.Where(d => d.NhanSuId == nv.Id).Sum(d => d.SoNgayNghi)
            })
            .OrderBy(x => x.PhongBan)
            .ThenBy(x => x.HoTen)
            .ToList();

            return reportData;
        }

        public async Task<IActionResult> LeaveReport(int? nam, string xemTheo = "Nam", string phongBan = "")
        {
            int reportYear = nam ?? DateTime.Now.Year;

            ViewBag.DanhSachPhongBan = await _context.PhongBans.Select(p => p.TenPhongBan).Distinct().ToListAsync();
            ViewBag.PhongBanDaChon = phongBan;
            ViewBag.Nam = reportYear;
            ViewBag.XemTheo = xemTheo;

            var reportData = await GetDataNghiPhep(reportYear, phongBan);
            return View(reportData);
        }

        public async Task<IActionResult> ExportLeaveReport(int? nam, string xemTheo = "Nam", string phongBan = "")
        {
            int reportYear = nam ?? DateTime.Now.Year;
            var reportData = await GetDataNghiPhep(reportYear, phongBan);

            return ExportExcelChung(
                data: reportData,
                sheetName: $"NghiPhep_{reportYear}",
                fileName: $"TinhHinhNghiPhep_{reportYear}.xlsx",
                headers: new[] { "STT", "Mã nhân viên", "Tên nhân viên", "Đơn vị công tác", "Số NP năm trước chuyển sang (6)", "Số NP trong năm (7)", "Số NP tăng theo thâm niên (8)", "Tổng (9)=(6+7+8)", "Đã nghỉ", "Còn lại" },
                fillDataRow: (ws, row, item, stt) =>
                {
                    ws.Cell(row, 1).Value = stt;
                    ws.Cell(row, 2).Value = item.MaNV;
                    ws.Cell(row, 3).Value = item.HoTen;
                    ws.Cell(row, 4).Value = item.PhongBan;
                    ws.Cell(row, 5).Value = item.PhepNamTruoc;
                    ws.Cell(row, 6).Value = item.PhepTrongNam;
                    ws.Cell(row, 7).Value = item.PhepThamNien;
                    ws.Cell(row, 8).Value = item.TongPhep;
                    ws.Cell(row, 9).Value = item.DaNghi;
                    ws.Cell(row, 10).Value = item.ConLai;
                }
            );
        }
        #endregion

        #region BÁO CÁO 4: TỔNG HỢP SỐ GIỜ LÀM VIỆC

        // 1. Hàm truy vấn và xử lý dữ liệu
        private async Task<List<SDHRM.Models.ViewModels.BaoCaoGioLamViecVM>> GetDataGioLamViec(DateTime startDate, DateTime endDate, string phongBan, string trangThai = "TatCa")
        {
            var query = GetBaseAttendanceQuery(startDate, endDate, phongBan);

            if (trangThai == "DangLamViec") query = query.Where(c => c.BangChamCongNhanVien.NhanSu.TrangThai == "Đang làm việc");
            else if (trangThai == "NghiViec") query = query.Where(c => c.BangChamCongNhanVien.NhanSu.TrangThai == "Đã nghỉ việc");

            var dataList = await query.ToListAsync();

            var reportData = dataList.GroupBy(c => new {
                c.BangChamCongNhanVien.NhanSu.Id,
                c.BangChamCongNhanVien.NhanSu.HoTen,
                TenPhongBan = c.BangChamCongNhanVien.NhanSu.PhongBan?.TenPhongBan,
                ViTri = c.BangChamCongNhanVien.NhanSu.ViTriCongViec?.TenViTri
            }).Select(g => new SDHRM.Models.ViewModels.BaoCaoGioLamViecVM
            {
                MaNV = g.Key.Id,
                HoTen = g.Key.HoTen,
                PhongBan = g.Key.TenPhongBan ?? "Chưa cập nhật",
                ViTriCongViec = g.Key.ViTri ?? "Chưa cập nhật", 
                GioThucTe = Math.Round(g.Sum(c => c.SoCongGhiNhan * 8), 2), // 1 công = 8 tiếng
                GioLamThem = Math.Round(g.Sum(c => c.GioOT), 2)
            })
            .OrderBy(x => x.PhongBan)
            .ThenBy(x => x.HoTen)
            .ToList();

            return reportData;
        }

        public async Task<IActionResult> WorkingHoursReport(DateTime? tuNgay, DateTime? denNgay, string phongBan = "", string trangThai = "TatCa")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date;
            DateTime endDate = denNgay ?? DateTime.Now.Date;

            ViewBag.DanhSachPhongBan = await _context.PhongBans.Select(p => p.TenPhongBan).Distinct().ToListAsync();
            ViewBag.PhongBanDaChon = phongBan;
            ViewBag.TuNgay = startDate;
            ViewBag.DenNgay = endDate;
            ViewBag.TrangThai = trangThai;

            var reportData = await GetDataGioLamViec(startDate, endDate, phongBan, trangThai);
            return View(reportData);
        }

        public async Task<IActionResult> ExportWorkingHoursReport(DateTime? tuNgay, DateTime? denNgay, string phongBan = "", string trangThai = "TatCa")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date;
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            var reportData = await GetDataGioLamViec(startDate, endDate, phongBan, trangThai);

            return ExportExcelChung(
                data: reportData,
                sheetName: "Số giờ làm việc",
                fileName: $"TongHopSoGioLamViec_{startDate:ddMMyyyy}_{endDate:ddMMyyyy}.xlsx",
                headers: new[] { "STT", "Mã nhân viên", "Họ và tên", "Đơn vị công tác", "Vị trí công việc", "Đi làm thực tế", "Làm thêm", "Tổng cộng" },
                fillDataRow: (ws, row, item, stt) =>
                {
                    ws.Cell(row, 1).Value = stt;
                    ws.Cell(row, 2).Value = item.MaNV;
                    ws.Cell(row, 3).Value = item.HoTen;
                    ws.Cell(row, 4).Value = item.PhongBan;
                    ws.Cell(row, 5).Value = item.ViTriCongViec;
                    ws.Cell(row, 6).Value = item.GioThucTe;
                    ws.Cell(row, 7).Value = item.GioLamThem;
                    ws.Cell(row, 8).Value = item.TongGio;
                }
            );
        }
        #endregion

        #region BÁO CÁO 5: DANH SÁCH NHÂN VIÊN LÀM THÊM GIỜ
        private async Task<List<SDHRM.Models.ViewModels.ChiTietLamThemVM>> GetDataChiTietLamThem(DateTime startDate, DateTime endDate, string phongBan)
        {
            // Gọi hàm lõi và chỉ lọc những ngày có phát sinh Giờ OT > 0
            var query = GetBaseAttendanceQuery(startDate, endDate, phongBan)
                        .Where(c => c.GioOT > 0);

            var dataList = await query.ToListAsync();

            var reportData = dataList.Select(c => new SDHRM.Models.ViewModels.ChiTietLamThemVM
            {
                MaNV = "NV000" + c.BangChamCongNhanVien.NhanSu.Id,
                HoTen = c.BangChamCongNhanVien.NhanSu.HoTen,
                ViTriCongViec = c.BangChamCongNhanVien.NhanSu.ViTriCongViec?.TenViTri ?? "Chưa cập nhật",
                PhongBan = c.BangChamCongNhanVien.NhanSu.PhongBan?.TenPhongBan ?? "Chưa cập nhật",
                NgayLamThem = c.Ngay,
                TongGio = Math.Round(c.GioOT, 2),                
                LyDo = c.LyDo ?? "Làm thêm giờ theo yêu cầu"
            })
            .OrderByDescending(x => x.NgayLamThem) // Sắp xếp ngày mới nhất lên đầu
            .ThenBy(x => x.PhongBan)
            .ThenBy(x => x.HoTen)
            .ToList();

            return reportData;
        }

        public async Task<IActionResult> OvertimeListReport(DateTime? tuNgay, DateTime? denNgay, string phongBan = "")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date;
            DateTime endDate = denNgay ?? DateTime.Now.Date;

            ViewBag.DanhSachPhongBan = await _context.PhongBans.Select(p => p.TenPhongBan).Distinct().ToListAsync();
            ViewBag.PhongBanDaChon = phongBan;
            ViewBag.TuNgay = startDate;
            ViewBag.DenNgay = endDate;

            var reportData = await GetDataChiTietLamThem(startDate, endDate, phongBan);
            return View(reportData);
        }

        public async Task<IActionResult> ExportOvertimeListReport(DateTime? tuNgay, DateTime? denNgay, string phongBan = "")
        {
            DateTime startDate = tuNgay ?? DateTime.Now.Date;
            DateTime endDate = denNgay ?? DateTime.Now.Date;
            var reportData = await GetDataChiTietLamThem(startDate, endDate, phongBan);

            // Tận dụng hàm xuất Excel dùng chung với 1 dòng code duy nhất!
            return ExportExcelChung(
                data: reportData,
                sheetName: "DS Làm thêm giờ",
                fileName: $"DanhSachLamThemGio_{startDate:ddMMyyyy}_{endDate:ddMMyyyy}.xlsx",
                headers: new[] { "STT", "Mã nhân viên", "Tên nhân viên", "Vị trí công việc", "Đơn vị công tác", "Ngày làm thêm", "Tổng giờ", "Lý do làm thêm" },
                fillDataRow: (ws, row, item, stt) =>
                {
                    ws.Cell(row, 1).Value = stt;
                    ws.Cell(row, 2).Value = item.MaNV;
                    ws.Cell(row, 3).Value = item.HoTen;
                    ws.Cell(row, 4).Value = item.ViTriCongViec;
                    ws.Cell(row, 5).Value = item.PhongBan;
                    ws.Cell(row, 6).Value = item.NgayLamThem.ToString("dd/MM/yyyy");
                    ws.Cell(row, 7).Value = item.TongGio;
                    ws.Cell(row, 8).Value = item.LyDo;
                }
            );
        }
        #endregion
    }
}