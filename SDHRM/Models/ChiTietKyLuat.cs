using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class ChiTietKyLuat
    {
        [Key]
        public int Id { get; set; }

        public int KyLuatId { get; set; }
        [ForeignKey("KyLuatId")]
        public virtual KyLuat? KyLuat { get; set; }

        [Required]
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }

        public string? HinhThucXuLy { get; set; }

        public decimal? SoTienPhat { get; set; } 

        public string? LyDoChiTiet { get; set; }

        [Required]
        public string TrangThai { get; set; } = "Chưa xử lý";
    }
}