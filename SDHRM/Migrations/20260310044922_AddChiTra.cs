using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddChiTra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuChiLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieuChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BangLuongId = table.Column<int>(type: "int", nullable: false),
                    KyTraLuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTienChi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhThucChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuChiLuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuChiLuongs_BangLuongs_BangLuongId",
                        column: x => x.BangLuongId,
                        principalTable: "BangLuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhieuChiLuongs_BangLuongId",
                table: "PhieuChiLuongs",
                column: "BangLuongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuChiLuongs");
        }
    }
}
