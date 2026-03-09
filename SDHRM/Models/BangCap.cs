using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SDHRM.Models
{
    public class BangCap
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }
        public string NoiDaoTao { get; set; }
        public int? TuNam { get; set; }
        public int? DenNam { get; set; }
        public string? Khoa { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? TrinhDoDaoTao { get; set; }
        public string? HinhThuc { get; set; }
        public string? XepLoai { get; set; }
        public bool DaTotNghiep { get; set; } = false;
        public DateTime? NgayNhanBang { get; set; }
        public string? TepDinhKem { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
