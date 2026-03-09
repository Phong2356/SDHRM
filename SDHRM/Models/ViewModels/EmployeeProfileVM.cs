namespace SDHRM.Models.ViewModels
{
    public class EmployeeProfileVM
    {
        public int Id { get; set; }
        public int MaNV { get; set; }
        public string HoTen { get; set; }

        // 2 trường này dùng để vẽ Avatar hình tròn có chữ cái đầu
        public string Initials { get; set; }
        public string AvatarColor { get; set; }
        public string AnhDaiDien { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string ViTri { get; set; }
        public string PhongBan { get; set; }
    }
}
