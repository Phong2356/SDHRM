namespace SDHRM.Models
{
    public class ThongTinCongViec
    {
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        public NhanSu? NhanSu { get; set; }
        public string? TrangThaiLaoDong { get; set; }
        public string? TinhChatLaoDong { get; set; }
        public string? LoaiHopDong { get; set; }
        public string? MaChamCong { get; set; }
        public DateTime? NgayHocViec { get; set; }
        public DateTime? NgayThuViec { get; set; }
        public DateTime? NgayChinhThuc { get; set; }
        public string? ThamNien { get; set; }
        public DateTime? NgayNghiHuuDuKien { get; set; }
    }
}
