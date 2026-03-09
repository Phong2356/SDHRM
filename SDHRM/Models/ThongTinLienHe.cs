using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class ThongTinLienHe
    {
        [Key]
        public int Id { get; set; }
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public NhanSu? NhanSu { get; set; }
        [Phone]
        public string? DTCoQuan { get; set; }
        [Phone]
        public string? DTNhaRieng { get; set; }
        [Phone]
        public string? DTKhac { get; set; }
        public string? EmailCoQuan { get; set; }
        public string? EmailKhac { get; set; }
        public string? LinkMangXaHoi { get; set; }
        public string? DiaChiThuongTru { get; set; }
        public string? DiaChiTamTru { get; set; }
        public string? HTLienHeKhanCap { get; set; }
        [Phone]
        public string? DTLienHeKhanCap { get; set; }
        public string? EmailLienHeKhanCap { get; set; }
        public string? DiaChiLienHeKhanCap { get; set; }
    }
}
