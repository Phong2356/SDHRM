using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class ChungChi
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }
        public string NhomChungChi { get; set; } = "";
        public string TenChungChi { get; set; } = "";
        public string? SoChungChi { get; set; }
        public string? TrinhDoDaoTao { get; set; }
        public DateTime? NgayCap { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string? NoiCap { get; set; }
        public string? DiemSo { get; set; }
        public string? XepLoai { get; set; }
        public string? TepDinhKem { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}