using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class KyLuat
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên kỷ luật/sự cố không được để trống")]
        public string TenKyLuat { get; set; } = "";
        [Required(ErrorMessage = "Loại kỷ luật/sự cố không được để trống")]
        public string LoaiKyLuat { get; set; } = "";
        public DateTime? NgayXayRa { get; set; }
        public string? NoiXayRa { get; set; }
        public string? NguyenNhan { get; set; }
        public string? MoTa { get; set; }
        public decimal? TongGiaTriThietHai { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn đơn vị liên quan")]
        public int PhongBanId { get; set; }
        [ForeignKey("PhongBanId")]
        public virtual PhongBan? PhongBan { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string TrangThai { get; set; } = "Chưa xử lý";
        public string? TepDinhKem { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public virtual ICollection<ChiTietKyLuat>? ChiTietKyLuats { get; set; }
    }
}