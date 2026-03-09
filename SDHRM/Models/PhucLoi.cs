namespace SDHRM.Models
{
    public class PhucLoi
    {
        public int Id { get; set; }
        public string TenChuongTrinh { get; set; }
        public string NhomPhucLoi { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc {  get; set; }
        public string? MucDich {  get; set; }
        public string? DiaDiem { get; set; }
        public string? HinhThucThucHien { get; set; }
        public string? TrangThai {  get; set; }
        public decimal? TienNVDongMoiNguoi { get; set; }
        public decimal? TienCongTyHoTroMoiNguoi { get; set; }
        public decimal? TongChiPhiMoiNguoi { get; set; }
        public decimal? TongTienNVDong { get; set; }
        public decimal? TongTienCongTyHoTro { get; set; }
        public decimal? TongChiPhi { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public virtual ICollection<PhucLoiNhanVien>? PhucLoiNhanViens { get; set; }
        public virtual ICollection<PhucLoiChiPhi>? PhucLoiChiPhis { get; set; }
    }
}
