using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class BangChamCongNhanVien
    {
        [Key]
        public int Id { get; set; }

        // Nối với Bảng chấm công (Cấp Cha)
        public int BangChamCongId { get; set; }
        [ForeignKey("BangChamCongId")]
        public virtual BangChamCong BangChamCong { get; set; }

        // Nối với Nhân viên
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }

        // === CÁC CỘT TỔNG HỢP ĐỂ TÍNH LƯƠNG ===
        public double TongCongThucTe { get; set; } = 0;
        public double TongCongNghiPhep { get; set; } = 0;
        public double TongCongKhongLuong { get; set; } = 0;
        public double TongCongLeTet { get; set; } = 0;
        public double TongGioTangCa { get; set; } = 0;
        public int TongPhutDiMuonVeSom { get; set; } = 0;

        // Bằng: Thực tế + Nghỉ Phép + Lễ Tết (Kế toán lấy cột này nhân tiền)
        public double TongCongHuongLuong { get; set; } = 0;

        public string? GhiChu { get; set; }

        // Nối với danh sách chi tiết 31 ngày (Cấp Cháu)
        public virtual ICollection<ChiTietChamCongNgay> ChiTietNgays { get; set; }
    }
}