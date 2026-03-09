namespace SDHRM.Models
{
    public class CauHinhWifi
    {
        public int Id { get; set; }
        public string TenVanPhong { get; set; } // Ví dụ: "Văn phòng Tầng 3", "Chi nhánh HCM"
        public string TenWifi { get; set; }     // Tên WiFi để hiển thị cho nhân viên biết
        public string DiaChiIP { get; set; }    // QUAN TRỌNG NHẤT: Public IP của mạng đó
        public bool TrangThai { get; set; } = true;
    }
}
