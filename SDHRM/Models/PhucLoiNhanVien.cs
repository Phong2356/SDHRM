using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class PhucLoiNhanVien
    {
        [Key]
        public int Id { get; set; }
        public int PhucLoiId { get; set; }
        [ForeignKey("PhucLoiId")]
        public virtual PhucLoi? PhucLoi { get; set; }
        [Required]
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }
        public string? GhiChu { get; set; }
    }
}
