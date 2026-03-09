using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class DonDiMuonVeSom
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }
        public required string LoaiDangKy { get; set; }
        [DataType(DataType.Date)]
        public DateTime NgayApDung { get; set; }        
        public required int SoPhut { get; set; }
        public required string LyDo { get; set; }
        public string TrangThai { get; set; } = "Chờ duyệt";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? NguoiDuyetId { get; set; }
        [ForeignKey("NguoiDuyetId")]
        public virtual NhanSu NguoiDuyet { get; set; }
        public string? GhiChuCuaNguoiDuyet { get; set; }
    }
}