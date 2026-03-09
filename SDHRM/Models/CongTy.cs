using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class CongTy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TenDayDu { get; set; } = null!;
        public string? TenVietTat { get; set; }
        public string? MaSoThue { get; set; }
        public DateTime? NgayThanhLap { get; set; }
        public string? Logo { get; set; }
        public string? SoGiayPhepDKKD { get; set; }
        public DateTime? NgayCap { get; set; }
        public string? NoiCap { get; set; }
        public string? DaiDienPhapLuat { get; set; }
        public string? DiaChi{ get; set; }
        public string? SoDienThoai { get; set; }
        public string? SoFax { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }
}
