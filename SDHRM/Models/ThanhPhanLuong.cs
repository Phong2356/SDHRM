using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SDHRM.Models
{
    public class ThanhPhanLuong
    {
        [Key]
        public int Id { get; set; }
        public required string MaThanhPhan { get; set; }
        public required string TenThanhPhan { get; set; }
        public string? LoaiThanhPhan { get; set; }

        public string? TinhChat { get; set; }

        public string? KieuGiaTri { get; set; }

        public string? GiaTri { get; set; }

        public bool KichHoat { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
