using System.Collections.Generic;

namespace SDHRM.Models
{
    public class DashboardViewModel
    {
        // 1. Chỉ số tổng quan
        public int TongNhanVien { get; set; }
        public int NhanVienChinhThuc { get; set; }
        public int NhanVienThuViec { get; set; }
        public int NhanVienNghiThaiSan { get; set; }
        public int NhanVienHocViec { get; set; }
        public int NhanVienKhac { get; set; }

        // 2. Chỉ số biến động (Tuần này vs Tuần trước)
        public int NhanVienMoiTuanNay { get; set; }
        public int ThuViecThanhCongTuanNay { get; set; }
        public int NghiViecTuanNay { get; set; }

        // 3. Nhắc việc
        public int ChuaKyHopDong { get; set; }

        // 4. Dữ liệu biểu đồ (Chuyển thành JSON ra View)
        public List<int> HopDongTheoLoai { get; set; } = new List<int>();
        public List<int> BienDongTiepNhan { get; set; } = new List<int>();
        public List<int> BienDongNghiViec { get; set; } = new List<int>();
        public List<int> SoLuongNhanSuThang { get; set; } = new List<int>();
        public List<string> PhongBanLabels { get; set; } = new List<string>();
        public List<int> PhongBanData { get; set; } = new List<int>();
    }
}