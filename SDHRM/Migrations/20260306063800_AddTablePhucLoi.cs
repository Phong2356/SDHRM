using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTablePhucLoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhucLois",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuongTrinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhomPhucLoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MucDich = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaDiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhThucThucHien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TienNVDongMoiNguoi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TienCongTyHoTroMoiNguoi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TongChiPhiMoiNguoi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TongTienNVDong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TongTienCongTyHoTro = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TongChiPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhucLois", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhucLoiChiPhis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhucLoiId = table.Column<int>(type: "int", nullable: false),
                    TenKhoanChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhucLoiChiPhis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhucLoiChiPhis_PhucLois_PhucLoiId",
                        column: x => x.PhucLoiId,
                        principalTable: "PhucLois",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhucLoiNhanViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhucLoiId = table.Column<int>(type: "int", nullable: false),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhucLoiNhanViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhucLoiNhanViens_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhucLoiNhanViens_PhucLois_PhucLoiId",
                        column: x => x.PhucLoiId,
                        principalTable: "PhucLois",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhucLoiChiPhis_PhucLoiId",
                table: "PhucLoiChiPhis",
                column: "PhucLoiId");

            migrationBuilder.CreateIndex(
                name: "IX_PhucLoiNhanViens_NhanSuId",
                table: "PhucLoiNhanViens",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_PhucLoiNhanViens_PhucLoiId",
                table: "PhucLoiNhanViens",
                column: "PhucLoiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhucLoiChiPhis");

            migrationBuilder.DropTable(
                name: "PhucLoiNhanViens");

            migrationBuilder.DropTable(
                name: "PhucLois");
        }
    }
}
