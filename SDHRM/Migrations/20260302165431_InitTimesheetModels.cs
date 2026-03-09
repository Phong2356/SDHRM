using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class InitTimesheetModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangChamCongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBangChamCong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonViApDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViTriApDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhNhanVienKiemNhiem = table.Column<bool>(type: "bit", nullable: false),
                    HinhThucChamCong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiCongChuan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangChamCongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BangChamCongNhanViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangChamCongId = table.Column<int>(type: "int", nullable: false),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    TongCongThucTe = table.Column<double>(type: "float", nullable: false),
                    TongCongNghiPhep = table.Column<double>(type: "float", nullable: false),
                    TongCongKhongLuong = table.Column<double>(type: "float", nullable: false),
                    TongCongLeTet = table.Column<double>(type: "float", nullable: false),
                    TongGioTangCa = table.Column<double>(type: "float", nullable: false),
                    TongPhutDiMuonVeSom = table.Column<int>(type: "int", nullable: false),
                    TongCongHuongLuong = table.Column<double>(type: "float", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangChamCongNhanViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangChamCongNhanViens_BangChamCongs_BangChamCongId",
                        column: x => x.BangChamCongId,
                        principalTable: "BangChamCongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BangChamCongNhanViens_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietChamCongNgays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangChamCongNhanVienId = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioVao = table.Column<TimeSpan>(type: "time", nullable: true),
                    GioRa = table.Column<TimeSpan>(type: "time", nullable: true),
                    PhutDiMuon = table.Column<int>(type: "int", nullable: false),
                    PhutVeSom = table.Column<int>(type: "int", nullable: false),
                    GioOT = table.Column<double>(type: "float", nullable: false),
                    SoCongGhiNhan = table.Column<double>(type: "float", nullable: false),
                    KyHieuChamCong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietChamCongNgays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietChamCongNgays_BangChamCongNhanViens_BangChamCongNhanVienId",
                        column: x => x.BangChamCongNhanVienId,
                        principalTable: "BangChamCongNhanViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangChamCongNhanViens_BangChamCongId",
                table: "BangChamCongNhanViens",
                column: "BangChamCongId");

            migrationBuilder.CreateIndex(
                name: "IX_BangChamCongNhanViens_NhanSuId",
                table: "BangChamCongNhanViens",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietChamCongNgays_BangChamCongNhanVienId",
                table: "ChiTietChamCongNgays",
                column: "BangChamCongNhanVienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietChamCongNgays");

            migrationBuilder.DropTable(
                name: "BangChamCongNhanViens");

            migrationBuilder.DropTable(
                name: "BangChamCongs");
        }
    }
}
