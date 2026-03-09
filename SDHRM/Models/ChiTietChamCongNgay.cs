using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class ChiTietChamCongNgay
    {
        [Key]
        public int Id { get; set; }

        // Nối với Tổng hợp tháng của NV (Cấp Con)
        public int BangChamCongNhanVienId { get; set; }
        [ForeignKey("BangChamCongNhanVienId")]
        public virtual BangChamCongNhanVien BangChamCongNhanVien { get; set; }

        [DataType(DataType.Date)]
        public DateTime Ngay { get; set; } // Ví dụ: 01/03/2026

        // Giờ In/Out lấy từ máy chấm công hoặc Đơn cập nhật công
        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }

        public int PhutDiMuon { get; set; } = 0;
        public int PhutVeSom { get; set; } = 0;
        public double GioOT { get; set; } = 0;

        // Số công ghi nhận của ngày này (1, 0.5, hoặc 0)
        public double SoCongGhiNhan { get; set; } = 0;

        // Ký hiệu hiển thị trên lưới (X: Đi làm, P: Nghỉ phép, V: Vắng, KL: Không lương...)
        public string KyHieuChamCong { get; set; } = "";

        // Lý do (Nếu có đơn xin nghỉ/đi muộn thì móc nội dung vào đây)
        public string? LyDo { get; set; }
    }
}