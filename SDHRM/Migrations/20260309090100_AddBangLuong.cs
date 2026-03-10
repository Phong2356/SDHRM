using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddBangLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBangLuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    BangChamCongId = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongQuyLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangLuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangLuongs_BangChamCongs_BangChamCongId",
                        column: x => x.BangChamCongId,
                        principalTable: "BangChamCongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietBangLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangLuongId = table.Column<int>(type: "int", nullable: false),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    LuongCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LuongDongBaoHiem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CongHuongLuong = table.Column<double>(type: "float", nullable: false),
                    LuongThucTe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhụCap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LuongOT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongThuNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TruBHXH = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TruBHYT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TruBHTN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TruThueTNCN = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhatDiMuon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongKhauTru = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThucLinh = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietBangLuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietBangLuongs_BangLuongs_BangLuongId",
                        column: x => x.BangLuongId,
                        principalTable: "BangLuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietBangLuongs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangLuongs_BangChamCongId",
                table: "BangLuongs",
                column: "BangChamCongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBangLuongs_BangLuongId",
                table: "ChiTietBangLuongs",
                column: "BangLuongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBangLuongs_NhanSuId",
                table: "ChiTietBangLuongs",
                column: "NhanSuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietBangLuongs");

            migrationBuilder.DropTable(
                name: "BangLuongs");
        }
    }
}
