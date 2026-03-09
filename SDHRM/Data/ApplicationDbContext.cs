using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SDHRM.Models;

namespace SDHRM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<NhanSu> NhanSus { get; set; }
        public DbSet<PhongBan> PhongBans { get; set; }
        public DbSet<ViTriCongViec> ViTriCongViecs { get; set; }
        public DbSet<CongTy> CongTys { get; set; }
        public DbSet<ThongTinChung> ThongTinChungs { get; set; }
        public DbSet<ThongTinLienHe> ThongTinLienHes { get; set; }
        public DbSet<ThongTinCongViec> ThongTinCongViecs { get; set; }
        public DbSet<KinhNghiemLamViec> KinhNghiemLamViecs { get; set; }
        public DbSet<CauHinhWifi> CauHinhWifis { get; set; }
        public DbSet<LichSuChamCong> LichSuChamCongs { get; set; }
        public DbSet<LoaiNghiPhep> LoaiNghiPheps { get; set; }
        public DbSet<QuyPhepNhanVien> QuyPhepNhanViens { get; set; }
        public DbSet<DonXinNghi> DonXinNghis { get; set; }
        public DbSet<DonDiMuonVeSom> DonDiMuonVeSoms { get; set; }
        public DbSet<DeNghiCapNhatCong> DeNghiCapNhatCongs { get; set; }
        public DbSet<DonTangCa> DonTangCas { get; set; }
        public DbSet<BangChamCong> BangChamCongs { get; set; }
        public DbSet<BangChamCongNhanVien> BangChamCongNhanViens { get; set; }
        public DbSet<ChiTietChamCongNgay> ChiTietChamCongNgays { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<KhenThuong> KhenThuongs { get; set; }
        public DbSet<ChiTietKhenThuong> ChiTietKhenThuongs { get; set; }
        public DbSet<KyLuat> KyLuats { get; set; }
        public DbSet<ChiTietKyLuat> ChiTietKyLuats { get; set; }
        public DbSet<PhucLoi> PhucLois { get; set; }
        public DbSet<PhucLoiNhanVien> PhucLoiNhanViens { get; set; }
        public DbSet<PhucLoiChiPhi> PhucLoiChiPhis { get; set; }
        public DbSet<BangCap> BangCaps { get; set; }
        public DbSet<ChungChi> ChungChis { get; set; }
        public DbSet<QuaTrinhCongTac> QuaTrinhCongTacs { get; set; }
        public DbSet<ThanhPhanLuong> ThanhPhanLuongs { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoaiNghiPhep>().HasData(
                new LoaiNghiPhep { Id = 1, TenLoai = "Nghỉ phép năm", TruVaoQuyPhep = true, CoHuongLuong = true, KichHoat = true },
                new LoaiNghiPhep { Id = 2, TenLoai = "Nghỉ không lương", TruVaoQuyPhep = false, CoHuongLuong = false, KichHoat = true },
                new LoaiNghiPhep { Id = 3, TenLoai = "Nghỉ ốm hưởng BHXH", TruVaoQuyPhep = false, CoHuongLuong = true, KichHoat = true },
                new LoaiNghiPhep { Id = 4, TenLoai = "Nghỉ thai sản", TruVaoQuyPhep = false, CoHuongLuong = true, KichHoat = true },
                new LoaiNghiPhep { Id = 5, TenLoai = "Nghỉ bù", TruVaoQuyPhep = false, CoHuongLuong = true, KichHoat = true },
                new LoaiNghiPhep { Id = 6, TenLoai = "Nghỉ kết hôn/ma chay", TruVaoQuyPhep = false, CoHuongLuong = true, KichHoat = true }
            );

            modelBuilder.Entity<QuaTrinhCongTac>()
                .HasOne(q => q.NhanSu)
                .WithMany(n => n.QuaTrinhCongTacs)
                .HasForeignKey(q => q.NhanSuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuaTrinhCongTac>()
                .HasOne(q => q.PhongBan)
                .WithMany()
                .HasForeignKey(q => q.PhongBanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuaTrinhCongTac>()
                .HasOne(q => q.ViTriCongViec)
                .WithMany()
                .HasForeignKey(q => q.ViTriCongViecId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuaTrinhCongTac>()
                .HasOne(q => q.QuanLyTrucTiep)
                .WithMany()
                .HasForeignKey(q => q.QuanLyTrucTiepId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChungChi>()
                .HasOne(c => c.NhanSu)
                .WithMany(n => n.ChungChis)
                .HasForeignKey(c => c.NhanSuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BangCap>()
                .HasOne(b => b.NhanSu)
                .WithMany(n => n.BangCaps)
                .HasForeignKey(b => b.NhanSuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.PhongBan)
                .WithMany()
                .HasForeignKey(h => h.PhongBanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NhanSu)
                .WithMany()
                .HasForeignKey(h => h.NhanSuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NguoiDaiDien)
                .WithMany()
                .HasForeignKey(h => h.NguoiDaiDienId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KhenThuong>()
                .HasOne(k => k.PhongBan)
                .WithMany()
                .HasForeignKey(k => k.PhongBanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Chặn xóa tự động: Xóa Nhân sự (Người quyết định) KHÔNG ĐƯỢC xóa Đợt khen thưởng
            modelBuilder.Entity<KhenThuong>()
                .HasOne(k => k.NguoiQuyetDinh)
                .WithMany()
                .HasForeignKey(k => k.NguoiQuyetDinhId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietKhenThuong>()
                .HasOne(c => c.KhenThuong)
                .WithMany(k => k.ChiTietKhenThuongs) 
                .HasForeignKey(c => c.KhenThuongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KyLuat>()
                .HasOne(k => k.PhongBan)
                .WithMany()
                .HasForeignKey(k => k.PhongBanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Không cho phép xóa Nhân sự nếu họ nằm trong danh sách kỷ luật
            modelBuilder.Entity<ChiTietKyLuat>()
                .HasOne(c => c.NhanSu)
                .WithMany(n => n.ChiTietKyLuats)
                .HasForeignKey(c => c.NhanSuId)
                .OnDelete(DeleteBehavior.Restrict);

            // NẾU xóa biên bản Sự cố chính -> Tự động xóa danh sách nhân viên bên trong
            modelBuilder.Entity<ChiTietKyLuat>()
                .HasOne(c => c.KyLuat)
                .WithMany(k => k.ChiTietKyLuats)
                .HasForeignKey(c => c.KyLuatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Không cho phép xóa Nhân sự nếu họ đang nằm trong list tham gia Phúc lợi
            modelBuilder.Entity<PhucLoiNhanVien>()
                .HasOne(pn => pn.NhanSu)
                .WithMany()
                .HasForeignKey(pn => pn.NhanSuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Xóa CT Phúc lợi -> Xóa toàn bộ danh sách Nhân viên tham gia
            modelBuilder.Entity<PhucLoiNhanVien>()
                .HasOne(pn => pn.PhucLoi)
                .WithMany(p => p.PhucLoiNhanViens)
                .HasForeignKey(pn => pn.PhucLoiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Xóa CT Phúc lợi -> Xóa toàn bộ danh sách Chi phí
            modelBuilder.Entity<PhucLoiChiPhi>()
                .HasOne(pc => pc.PhucLoi)
                .WithMany(p => p.PhucLoiChiPhis)
                .HasForeignKey(pc => pc.PhucLoiId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
