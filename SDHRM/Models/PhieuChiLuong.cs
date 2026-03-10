using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class PhieuChiLuong
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string MaPhieuChi { get; set; }
        [Required]
        public int BangLuongId { get; set; }
        [ForeignKey("BangLuongId")]
        public virtual BangLuong BangLuong { get; set; }
        [Required]
        public string KyTraLuong { get; set; } 
        public decimal SoTienChi { get; set; }
        public string HinhThucChi { get; set; } 
        [DataType(DataType.Date)]
        public DateTime NgayThanhToan { get; set; }
        public string TrangThai { get; set; } = "Bản nháp";
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public virtual ICollection<ChiTietPhieuChiLuong> ChiTietPhieuChiLuongs { get; set; }
    }
}