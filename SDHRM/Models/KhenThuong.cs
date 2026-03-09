using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class KhenThuong
    {
        [Key]
        public int Id { get; set; }
        public required string TenDotKhenThuong { get; set; }
        public DateTime? NgayKhenThuong { get; set; }
        public string? SoQuyetDinh { get; set; }
        public int? NguoiQuyetDinhId { get; set; }
        [ForeignKey("NguoiQuyetDinhId")]
        public virtual NhanSu? NguoiQuyetDinh { get; set; }
        public string? HinhThucKhenThuong { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn đối tượng khen thưởng")]
        public string? DoiTuongKhenThuong { get; set; }
        public string? CapKhenThuong { get; set; }
        public string? LyDo { get; set; }
        public int PhongBanId { get; set; }
        [ForeignKey("PhongBanId")]
        public virtual PhongBan? PhongBan { get; set; }
        public required string LoaiKhenThuong { get; set; }
        public DateTime NgayQuyetDinh { get; set; }
        public string? ChucDanhNguoiQuyetDinh { get; set; }
        public decimal? TongGiaTri { get; set; }
        public string? ChiTietDoiTuong { get; set; }
        public string? CanCu { get; set; }
        public required string TrangThai { get; set; }
        public string? TepDinhKem { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public ICollection<ChiTietKhenThuong>? ChiTietKhenThuongs { get; set; }
    }
}
