namespace SDHRM.Models.Constants
{
    public static class Permissions
    {
        public static List<string> GenerateAllPermissions()
        {
            return new List<string>
            {
                // Quyền khu vực Tiền lương
                "Payroll.View",
                "Payroll.Edit",
                "Payroll.Approve",

                // Quyền khu vực Chấm công
                "Timesheet.View",
                "Timesheet.Edit",
                "Timesheet.Approve",

                // Quyền khu vực Hồ sơ Nhân sự
                "EmployeeInfo.View",
                "EmployeeInfo.Edit",

                // Quyền Hệ thống
                "Systems.Manage"
            };
        }
    }

    public class RoleClaimsViewModel
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool IsSelected { get; set; } 
    }

    public class ManageRoleClaimsViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaimsViewModel> Claims { get; set; }
    }
}
