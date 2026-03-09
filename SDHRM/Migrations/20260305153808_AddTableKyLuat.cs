using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTableKyLuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KyLuats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKyLuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiKyLuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayXayRa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiXayRa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguyenNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TongGiaTriThietHai = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PhongBanId = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TepDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KyLuats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KyLuats_PhongBans_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKyLuats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KyLuatId = table.Column<int>(type: "int", nullable: false),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    HinhThucXuLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTienPhat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LyDoChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKyLuats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietKyLuats_KyLuats_KyLuatId",
                        column: x => x.KyLuatId,
                        principalTable: "KyLuats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietKyLuats_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKyLuats_KyLuatId",
                table: "ChiTietKyLuats",
                column: "KyLuatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKyLuats_NhanSuId",
                table: "ChiTietKyLuats",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_KyLuats_PhongBanId",
                table: "KyLuats",
                column: "PhongBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietKyLuats");

            migrationBuilder.DropTable(
                name: "KyLuats");
        }
    }
}
