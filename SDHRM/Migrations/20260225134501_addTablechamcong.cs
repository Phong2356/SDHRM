using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class addTablechamcong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhWifis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVanPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenWifi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhWifis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichSuChamCongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    ThoiGianCham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiChamCong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuChamCongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichSuChamCongs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChamCongs_NhanSuId",
                table: "LichSuChamCongs",
                column: "NhanSuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhWifis");

            migrationBuilder.DropTable(
                name: "LichSuChamCongs");
        }
    }
}
