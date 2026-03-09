using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBangCap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKhenThuongs_KhenThuongs_KhenThuongId",
                table: "ChiTietKhenThuongs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKhenThuongs_NhanSus_NhanSuId",
                table: "ChiTietKhenThuongs");

            migrationBuilder.CreateTable(
                name: "BangCaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    NoiDaoTao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TuNam = table.Column<int>(type: "int", nullable: true),
                    DenNam = table.Column<int>(type: "int", nullable: true),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChuyenNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrinhDoDaoTao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhThuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XepLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaTotNghiep = table.Column<bool>(type: "bit", nullable: false),
                    NgayNhanBang = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TepDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangCaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangCaps_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangCaps_NhanSuId",
                table: "BangCaps",
                column: "NhanSuId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKhenThuongs_KhenThuongs_KhenThuongId",
                table: "ChiTietKhenThuongs",
                column: "KhenThuongId",
                principalTable: "KhenThuongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKhenThuongs_NhanSus_NhanSuId",
                table: "ChiTietKhenThuongs",
                column: "NhanSuId",
                principalTable: "NhanSus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKhenThuongs_KhenThuongs_KhenThuongId",
                table: "ChiTietKhenThuongs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKhenThuongs_NhanSus_NhanSuId",
                table: "ChiTietKhenThuongs");

            migrationBuilder.DropTable(
                name: "BangCaps");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKhenThuongs_KhenThuongs_KhenThuongId",
                table: "ChiTietKhenThuongs",
                column: "KhenThuongId",
                principalTable: "KhenThuongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKhenThuongs_NhanSus_NhanSuId",
                table: "ChiTietKhenThuongs",
                column: "NhanSuId",
                principalTable: "NhanSus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
