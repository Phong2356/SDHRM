using System;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class BangChamCong
    {
        [Key]
        public int Id { get; set; }
        public required string TenBangChamCong { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string? DonViApDung { get; set; } = "Tất cả đơn vị";
        public string? ViTriApDung { get; set; } = "Tất cả vị trí";
        public string CachTinhCong { get; set; } = "Số công chuẩn - Nghỉ không lương";
        public string TrangThai { get; set; } = "Chưa khóa";
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}