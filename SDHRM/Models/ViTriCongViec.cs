using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class ViTriCongViec : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string TenViTri { get; set; }
        public string? NhomCongViec { get; set; }
        public string TrangThai { get; set; }
    }
}
