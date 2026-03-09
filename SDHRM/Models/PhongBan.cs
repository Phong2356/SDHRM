using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class PhongBan : IBaseEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        public string TenPhongBan { get; set; }
        public string? KyHieu { get; set; }
        public int? IDTruongPhong { get; set; }
        [ForeignKey("IDTruongPhong")]
        public NhanSu? TruongPhong { get; set; }
        public string? MoTa { get; set; }
        public string TrangThai { get; set; }
        [InverseProperty("PhongBan")]
        public ICollection<NhanSu>? NhanSus { get; set; }
    }
}
