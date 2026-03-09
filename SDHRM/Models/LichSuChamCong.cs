namespace SDHRM.Models
{
    public class LichSuChamCong
    {
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        public NhanSu? NhanSu { get; set; }
        public DateTime ThoiGianCham { get; set; }
        public string? AnhChamCong { get; set; }
        public string LoaiChamCong { get; set; } 
        public string IPNguoiDung { get; set; } 
        public string TrangThai { get; set; }  
    }
}
