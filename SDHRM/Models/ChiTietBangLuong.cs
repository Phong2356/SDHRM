using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class ChiTietBangLuong
    {
        [Key]
        public int Id { get; set; }

        public int BangLuongId { get; set; }
        [ForeignKey("BangLuongId")]
        public virtual BangLuong? BangLuong { get; set; }

        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }
        public decimal LuongCoBan { get; set; } = 0;
        public decimal LuongDongBaoHiem { get; set; } = 0;
        public double CongHuongLuong { get; set; } = 0;
        public decimal TongThuNhap { get; set; } = 0;
        public decimal TongKhauTru { get; set; } = 0;
        public decimal ThucLinh { get; set; } = 0;
        public string TrangThaiXacNhan { get; set; } = "Chưa gửi";
        public string? ThacMac { get; set; }
        public virtual ICollection<KetQuaLuong>? KetQuaLuongs { get; set; }
    }
}