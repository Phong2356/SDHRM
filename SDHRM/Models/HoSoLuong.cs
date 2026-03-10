using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDHRM.Models
{
    public class HoSoLuong
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NhanSuId { get; set; }
        [ForeignKey("NhanSuId")]
        public virtual NhanSu? NhanSu { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Lương cơ bản")]
        public decimal LuongCoBan { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng nhập Lương đóng bảo hiểm")]
        public decimal LuongDongBaoHiem { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng nhập Số người phụ thuộc")]
        public int SoNguoiPhuThuoc { get; set; } = 0;
        public decimal PhuCapChucVu { get; set; } = 0;
        public decimal PhuCapDiLai { get; set; } = 0;
        public decimal PhuCapKhac { get; set; } = 0;
        public string? GhiChu { get; set; }
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}