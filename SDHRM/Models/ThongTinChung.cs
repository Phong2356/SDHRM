using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class ThongTinChung
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public NhanSu? NhanSu { get; set; }
        public string? NoiSinh { get; set; }
        public string? NguyenQuan { get; set; }
        public string? TinhTrangHonNhan { get; set; }
        public string? MaSoThue { get; set; }
        public string? DanToc { get; set; }
        public string? TonGiao { get; set; }
        public string? QuocTich { get; set; }
        public string? SoCCCD { get; set; }
        public DateTime? NgayCapGiayTo { get; set; }
        public string? NoiCapGiayTo { get; set; }
        public DateTime? NgayHetHanGiayTo { get; set; }
        public string? SoHoChieu{ get; set; }
        public DateTime? NgayCapHoChieu { get; set; }
        public string? NoiCapHoChieu { get; set; }
        public DateTime? NgayHetHanHoChieu { get; set; }
        public string? TrinhDoHocVan { get; set; }
        public string? TrinhDoDaoTao { get; set; }
        public string? NoiDaoTao { get; set; }
        public string? Khoa { get; set; }
        public string? ChuyenNganh { get; set; }
        public string? NamTotNghiep { get; set; }
        public string? XepLoaiTotNghiep { get; set; }
    }
}
