using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddKhenThuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhenThuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDotKhenThuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayKhenThuong = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoQuyetDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguoiQuyetDinhId = table.Column<int>(type: "int", nullable: true),
                    HinhThucKhenThuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoiTuongKhenThuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapKhenThuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhongBanId = table.Column<int>(type: "int", nullable: false),
                    LoaiKhenThuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayQuyetDinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChucDanhNguoiQuyetDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TongGiaTri = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChiTietDoiTuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TepDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhenThuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KhenThuongs_NhanSus_NguoiQuyetDinhId",
                        column: x => x.NguoiQuyetDinhId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KhenThuongs_PhongBans_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKhenThuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhenThuongId = table.Column<int>(type: "int", nullable: false),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    GiaTriKhenThuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TepDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKhenThuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietKhenThuongs_KhenThuongs_KhenThuongId",
                        column: x => x.KhenThuongId,
                        principalTable: "KhenThuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietKhenThuongs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKhenThuongs_KhenThuongId",
                table: "ChiTietKhenThuongs",
                column: "KhenThuongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKhenThuongs_NhanSuId",
                table: "ChiTietKhenThuongs",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_KhenThuongs_NguoiQuyetDinhId",
                table: "KhenThuongs",
                column: "NguoiQuyetDinhId");

            migrationBuilder.CreateIndex(
                name: "IX_KhenThuongs_PhongBanId",
                table: "KhenThuongs",
                column: "PhongBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietKhenThuongs");

            migrationBuilder.DropTable(
                name: "KhenThuongs");
        }
    }
}
