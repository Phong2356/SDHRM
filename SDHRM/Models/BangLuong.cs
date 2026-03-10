using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class BangLuong
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenBangLuong { get; set; } = "";

        public int Thang { get; set; }
        public int Nam { get; set; }

        // Liên kết với bảng chấm công đã chốt
        public int BangChamCongId { get; set; }
        [ForeignKey("BangChamCongId")]
        public virtual BangChamCong? BangChamCong { get; set; }

        public string TrangThai { get; set; } = "Bản nháp"; // Bản nháp -> Chờ duyệt -> Đã duyệt -> Đã chi trả

        // Tổng hợp toàn công ty để báo cáo sếp
        public decimal TongQuyLuong { get; set; } = 0;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int MauBangLuongId { get; set; }
        [ForeignKey("MauBangLuongId")]
        public virtual MauBangLuong? MauBangLuong { get; set; }
        public virtual ICollection<ChiTietBangLuong>? ChiTietBangLuongs { get; set; }
    }
}