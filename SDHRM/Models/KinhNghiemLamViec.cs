using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class KinhNghiemLamViec
    {
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        public NhanSu? NhanSu { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public required string NoiLamViec { get; set; }
        public string? MucLuong { get; set; }
        public string? GhiChu { get; set; }
        public string? ChucVu { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public required string ViTriCongViec { get; set; }
        public string? MoTaCongViec { get; set; }
        public string? NguoiDoiChieu { get; set; }
        public string? TrangThai { get; set; }
    }
}
