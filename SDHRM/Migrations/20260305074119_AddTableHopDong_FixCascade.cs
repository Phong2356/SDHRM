using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTableHopDong_FixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HopDongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    PhongBanId = table.Column<int>(type: "int", nullable: false),
                    SoHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiHanHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhThucLamViec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCoHieuLuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LuongCoBan = table.Column<double>(type: "float", nullable: true),
                    LuongDongBaoHiem = table.Column<double>(type: "float", nullable: true),
                    TyLeHuongLuong = table.Column<double>(type: "float", nullable: false),
                    NguoiDaiDienId = table.Column<int>(type: "int", nullable: true),
                    ChucDanhNguoiDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TepDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiKy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HopDongs_NhanSus_NguoiDaiDienId",
                        column: x => x.NguoiDaiDienId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HopDongs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HopDongs_PhongBans_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoHuongLuong",
                value: true);

            migrationBuilder.UpdateData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 4,
                column: "CoHuongLuong",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_NguoiDaiDienId",
                table: "HopDongs",
                column: "NguoiDaiDienId");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_NhanSuId",
                table: "HopDongs",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_PhongBanId",
                table: "HopDongs",
                column: "PhongBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HopDongs");

            migrationBuilder.UpdateData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoHuongLuong",
                value: false);

            migrationBuilder.UpdateData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 4,
                column: "CoHuongLuong",
                value: false);
        }
    }
}
