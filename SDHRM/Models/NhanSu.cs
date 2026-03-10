using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class NhanSu : IBaseEntity
    {
        public int Id { get; set; }
        public string? AnhDaiDien { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public string? MaDinhDanh { get; set; }
        public int PhongBanId { get; set; }
        [ForeignKey("PhongBanId")]
        [InverseProperty("NhanSus")]
        public PhongBan? PhongBan { get; set; }
        public int ViTriCongViecId { get; set; }
        public ViTriCongViec? ViTriCongViec { get; set; }
        public int? QuanLyId { get; set; }
        [ForeignKey("QuanLyId")]
        public NhanSu? QuanLy { get; set; }
        public string TrangThai { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? SoDienThoai { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ThongTinChung? ThongTinChung { get; set; }
        public ThongTinLienHe? ThongTinLienHe { get; set; }
        public ThongTinCongViec? ThongTinCongViec { get; set; }
        public virtual ICollection<KinhNghiemLamViec>? KinhNghiemLamViecs { get; set; }
        public virtual ICollection<ChiTietKhenThuong>? ChiTietKhenThuongs { get; set; }
        public virtual ICollection<ChiTietKyLuat>? ChiTietKyLuats { get; set; }
        public virtual ICollection<BangCap>? BangCaps { get; set; }
        public virtual ICollection<ChungChi>? ChungChis { get; set; }
        public virtual ICollection<QuaTrinhCongTac>? QuaTrinhCongTacs { get; set; }
        public virtual HoSoLuong? HoSoLuong { get; set; }
    }
}

