using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class PhucLoiChiPhi
    {
        [Key]
        public int Id { get; set; }
        public int PhucLoiId { get; set; }
        [ForeignKey("PhucLoiId")]
        public virtual PhucLoi? PhucLoi { get; set; }
        [Required]
        public string TenKhoanChi { get; set; } = "";
        public decimal? SoTien { get; set; }
        public string? GhiChu { get; set; }
    }
}
