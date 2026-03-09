using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class QuyPhepNhanVien
    {
        [Key]
        public int Id { get; set; }

        // Sửa từ string thành int ở đây
        public int NhanSuId { get; set; }

        [ForeignKey("NhanSuId")]
        public virtual NhanSu NhanSu { get; set; }

        public int Nam { get; set; }
        public double TongPhepNam { get; set; }
        public double PhepTonNamTruoc { get; set; } = 0;
        public double PhepThamNien { get; set; } = 0;
        public double PhepThuong { get; set; } = 0;
        public double SoPhepDaDung { get; set; } = 0;

        [NotMapped]
        [Display(Name = "Số phép còn lại")]
        public double SoPhepConLai => TongPhepNam + PhepTonNamTruoc + PhepThamNien + PhepThuong - SoPhepDaDung;
    }
}