namespace SDHRM.Models.ViewModels
{
    public class MonthlyAttendanceViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();
    }

    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; } // Xác định có phải ngày của tháng đang chọn không
        public bool IsToday { get; set; }        // Xác định có phải hôm nay không
        public TimeSpan? GioVao { get; set; }    // Giờ Check-in
        public TimeSpan? GioRa { get; set; }     // Giờ Check-out
    }
}
