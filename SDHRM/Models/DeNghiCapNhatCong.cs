using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class DeNghiCapNhatCong
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày cần cập nhật")]
        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.Date)]
        public DateTime NgayCapNhat { get; set; }
        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }
        public required string LyDo { get; set; }
        public string? FileDinhKem { get; set; }
        public string TrangThai { get; set; } = "Chờ duyệt";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? NguoiDuyetId { get; set; }
        [ForeignKey("NguoiDuyetId")]
        public virtual NhanSu NguoiDuyet { get; set; }
        public string? GhiChuCuaNguoiDuyet { get; set; }
    }
}