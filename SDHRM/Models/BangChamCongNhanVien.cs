using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class BangChamCongNhanVien
    {
        [Key]
        public int Id { get; set; }
        public int BangChamCongId { get; set; }
        [ForeignKey("BangChamCongId")]
        public virtual BangChamCong BangChamCong { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }
        public double TongCongThucTe { get; set; } = 0;
        public double TongCongNghiPhep { get; set; } = 0;
        public double TongCongKhongLuong { get; set; } = 0;
        public double TongCongLeTet { get; set; } = 0;
        public double TongGioTangCa { get; set; } = 0;
        public int TongPhutDiMuonVeSom { get; set; } = 0;
        public double TongCongHuongLuong { get; set; } = 0;
        public string? GhiChu { get; set; }
        public virtual ICollection<ChiTietChamCongNgay> ChiTietNgays { get; set; }
    }
}