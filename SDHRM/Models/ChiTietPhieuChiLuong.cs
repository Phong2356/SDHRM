using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class ChiTietPhieuChiLuong
    {
        [Key]
        public int Id { get; set; }
        public int PhieuChiLuongId { get; set; }
        [ForeignKey("PhieuChiLuongId")]
        public virtual PhieuChiLuong PhieuChiLuong { get; set; }
        public int ChiTietBangLuongId { get; set; }
        [ForeignKey("ChiTietBangLuongId")]
        public virtual ChiTietBangLuong ChiTietBangLuong { get; set; }
        public decimal SoTienChi { get; set; }
    }
}