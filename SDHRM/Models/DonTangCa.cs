using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class DonTangCa
    {
        [Key]
        public int Id { get; set; }

        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }
        public DateTime NgayTangCa { get; set; }
        public TimeSpan TuGio { get; set; }
        public TimeSpan DenGio { get; set; }
        public double SoGio { get; set; }
        public required string LyDoTangCa { get; set; }
        public string TrangThai { get; set; } = "Chờ duyệt";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? NguoiDuyetId { get; set; }
        [ForeignKey("NguoiDuyetId")]
        public virtual NhanSu NguoiDuyet { get; set; }
        public string? GhiChuCuaNguoiDuyet { get; set; }
    }
}