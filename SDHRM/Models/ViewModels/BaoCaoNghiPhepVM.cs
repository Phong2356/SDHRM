namespace SDHRM.Models.ViewModels
{
    public class BaoCaoNghiPhepVM
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string PhongBan { get; set; }

        // Các cột phép theo quy định
        public double PhepNamTruoc { get; set; } // (6)
        public double PhepTrongNam { get; set; } // (7)
        public double PhepThamNien { get; set; } // (8)

        // Tổng số phép có trong năm
        public double TongPhep => PhepNamTruoc + PhepTrongNam + PhepThamNien; // (9)
        public double DaNghi { get; set; }
        public double ConLai => TongPhep - DaNghi;
    }
}
