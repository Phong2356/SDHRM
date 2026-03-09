using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class ChiTietKhenThuong
    {
        [Key]
        public int Id { get; set; }
        public int KhenThuongId { get; set; }
        [ForeignKey("KhenThuongId")]
        public virtual KhenThuong? KhenThuong { get; set; }
        [Required]
        public int NhanSuId { get; set; }
        public virtual NhanSu? NhanSu { get; set; }

        public decimal? GiaTriKhenThuong { get; set; }

        public string? LyDo { get; set; }

        public string? TepDinhKem { get; set; }

        [Required]
        public string TrangThai { get; set; } = "Chưa thực hiện";
    }
}
