using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddMauBangLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MauBangLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MacDinh = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MauBangLuongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietMauBangLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MauBangLuongId = table.Column<int>(type: "int", nullable: false),
                    ThanhPhanLuongId = table.Column<int>(type: "int", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietMauBangLuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietMauBangLuongs_MauBangLuongs_MauBangLuongId",
                        column: x => x.MauBangLuongId,
                        principalTable: "MauBangLuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietMauBangLuongs_ThanhPhanLuongs_ThanhPhanLuongId",
                        column: x => x.ThanhPhanLuongId,
                        principalTable: "ThanhPhanLuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietMauBangLuongs_MauBangLuongId",
                table: "ChiTietMauBangLuongs",
                column: "MauBangLuongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietMauBangLuongs_ThanhPhanLuongId",
                table: "ChiTietMauBangLuongs",
                column: "ThanhPhanLuongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietMauBangLuongs");

            migrationBuilder.DropTable(
                name: "MauBangLuongs");
        }
    }
}
