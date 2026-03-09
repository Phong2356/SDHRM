using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class DonXinNghi
    {
        [Key]
        public int Id { get; set; }

        // Sửa từ string thành int ở đây
        public int NhanSuId { get; set; }

        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }

        public int LoaiNghiPhepId { get; set; }

        [ForeignKey("LoaiNghiPhepId")]
        public virtual LoaiNghiPhep LoaiNghiPhep { get; set; }

        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public double SoNgayNghi { get; set; }
        public string LyDo { get; set; }
        public string TrangThai { get; set; } = "Chờ duyệt";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? NguoiDuyetId { get; set; }
        [ForeignKey("NguoiDuyetId")]
        public virtual NhanSu NguoiDuyet { get; set; }
        public string? FileDinhKem { get; set; }
        public string? GhiChuCuaNguoiDuyet { get; set; }
    }
}