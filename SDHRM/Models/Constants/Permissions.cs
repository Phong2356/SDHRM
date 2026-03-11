namespace SDHRM.Models.Constants
{
    public static class Permissions
    {
        public static List<string> GenerateAllPermissions()
        {
            return new List<string>
            {
                // 1. NHÓM QUYỀN HỆ THỐNG (Area: Systems)
                "Systems.Manage", 

                // 2. NHÓM QUYỀN HỒ SƠ NHÂN SỰ (Area: InfoEmployees)
                "EmployeeInfo.View",     // Chỉ xem danh sách nhân sự
                "EmployeeInfo.Manage",   // Thêm, Sửa hồ sơ nhân viên, quản lý phúc lợi
                "EmployeeInfo.Delete",   // Xóa hồ sơ (Nghỉ việc)
                "Contracts.View",        // Xem hợp đồng
                "Contracts.Manage",      // Tạo, gia hạn hợp đồng
                "RewardsDisciplines.Manage", // Quản lý Khen thưởng & Kỷ luật

                // 3. NHÓM QUYỀN CHẤM CÔNG (Area: Timesheet)
                "Timesheet.View",        // Xem bảng chấm công tổng hợp
                "Timesheet.Manage",      // Import công, sửa công tay, cài đặt ca làm
                "Timesheet.Approve",     // Chấp nhận/Từ chối các loại Đơn xin nghỉ, Tăng ca, Đi muộn về sớm

                // 4. NHÓM QUYỀN TIỀN LƯƠNG (Area: Payroll)
                "Payroll.View",          // Xem bảng lương các tháng
                "Payroll.Manage",        // Chạy tính lương, thiết lập thành phần/mẫu lương, tạo đợt thanh toán
                "Payroll.Approve",       // Chốt duyệt bảng lương cuối cùng

                // 5. NHÓM QUYỀN BÁO CÁO (Dành cho Cấp quản lý/Giám đốc)
                "Reports.View"           // Truy cập các biểu đồ, báo cáo tổng hợp
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
