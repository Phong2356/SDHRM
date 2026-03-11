namespace SDHRM.Models.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public int MaNV { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; } 
        public string PhongBan { get; set; }
        public string ViTriCongViec { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
    public class UserRolesViewModel
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public List<UserRolesViewModel> UserRoles { get; set; }
    }
}
