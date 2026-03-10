using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class MauBangLuong
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên mẫu bảng lương")]
        public string TenMau { get; set; } = "";
        public string? GhiChu { get; set; }
        public bool MacDinh { get; set; } = false;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public virtual ICollection<ChiTietMauBangLuong>? ChiTietMaus { get; set; }
    }

    public class ChiTietMauBangLuong
    {
        [Key]
        public int Id { get; set; }
        public int MauBangLuongId { get; set; }
        [ForeignKey("MauBangLuongId")]
        public virtual MauBangLuong? MauBangLuong { get; set; }
        public int ThanhPhanLuongId { get; set; }
        [ForeignKey("ThanhPhanLuongId")]
        public virtual ThanhPhanLuong? ThanhPhanLuong { get; set; }
        public int ThuTu { get; set; } = 0;
    }
}