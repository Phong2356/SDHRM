namespace SDHRM.Models.ViewModels
{
    public class BaoCaoGioLamViecVM
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string PhongBan { get; set; }
        public string ViTriCongViec { get; set; }
        public double GioThucTe { get; set; }
        public double GioLamThem { get; set; }
        public double TongGio => GioThucTe + GioLamThem;
    }
}
