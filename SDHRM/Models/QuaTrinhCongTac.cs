using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class QuaTrinhCongTac
    {
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }
        public DateTime TuNgay { get; set; }
        public int PhongBanId { get; set; }
        [ForeignKey("PhongBanId")]
        public virtual PhongBan? PhongBan { get; set; }
        public int ViTriCongViecId { get; set; }
        [ForeignKey("ViTriCongViecId")]
        public virtual ViTriCongViec? ViTriCongViec { get; set; }
        public string TinhChatLaoDong { get; set; } = "Chính thức";
        public int? QuanLyTrucTiepId { get; set; }
        [ForeignKey("QuanLyTrucTiepId")]
        public virtual NhanSu? QuanLyTrucTiep { get; set; }
        public string? SoQuyetDinh { get; set; }
        public DateTime? NgayQuyetDinh { get; set; }
        public string? TepDinhKem { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
