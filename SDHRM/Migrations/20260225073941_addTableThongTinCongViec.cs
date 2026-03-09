using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class addTableThongTinCongViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongTinCongViecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiLaoDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhChatLaoDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiHopDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaChamCong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayHocViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayThuViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayChinhThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThamNien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayNghiHuuDuKien = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinCongViecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongTinCongViecs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinCongViecs_NhanSuId",
                table: "ThongTinCongViecs",
                column: "NhanSuId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongTinCongViecs");
        }
    }
}
