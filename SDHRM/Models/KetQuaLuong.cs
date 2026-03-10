using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class KetQuaLuong
    {
        [Key]
        public int Id { get; set; }

        public int ChiTietBangLuongId { get; set; }
        [ForeignKey("ChiTietBangLuongId")]
        public virtual ChiTietBangLuong? ChiTietBangLuong { get; set; }

        public int ThanhPhanLuongId { get; set; }
        [ForeignKey("ThanhPhanLuongId")]
        public virtual ThanhPhanLuong? ThanhPhanLuong { get; set; }

        public decimal SoTien { get; set; } = 0;
    }
}