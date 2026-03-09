using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class LoaiNghiPhep
    {
        [Key]
        public int Id { get; set; }
        public string TenLoai { get; set; }
        public bool TruVaoQuyPhep { get; set; }
        public bool CoHuongLuong { get; set; }
        public bool KichHoat { get; set; } = true;
        public virtual ICollection<DonXinNghi> DanhSachDonXinNghi { get; set; }
    }
}
