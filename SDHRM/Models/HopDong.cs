using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class HopDong
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }
        public int PhongBanId { get; set; }
        [ForeignKey("PhongBanId")]
        public virtual PhongBan PhongBan { get; set; }

        // --- THÔNG TIN CHUNG CỦA HỢP ĐỒNG ---
        public string SoHopDong { get; set; }
        public string? TenHopDong { get; set; }
        public string ThoiHanHopDong { get; set; }
        public string LoaiHopDong { get; set; }
        public string HinhThucLamViec { get; set; }
        public DateTime NgayKy { get; set; }
        public DateTime NgayCoHieuLuc { get; set; }
        public DateTime NgayHetHan { get; set; }
        public double? LuongCoBan { get; set; }
        public double? LuongDongBaoHiem { get; set; }
        public double TyLeHuongLuong { get; set; } = 100; 
        // --- NGƯỜI ĐẠI DIỆN ---
        public int? NguoiDaiDienId { get; set; }
        [ForeignKey("NguoiDaiDienId")]
        public virtual NhanSu NguoiDaiDien { get; set; }
        public string ChucDanhNguoiDaiDien { get; set; }
        // --- THÔNG TIN KHÁC ---
        public string? GhiChu { get; set; }
        public string? TepDinhKem { get; set; } 
        public string TrangThaiKy { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}