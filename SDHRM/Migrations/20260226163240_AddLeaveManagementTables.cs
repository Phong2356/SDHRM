using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiNghiPheps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TruVaoQuyPhep = table.Column<bool>(type: "bit", nullable: false),
                    CoHuongLuong = table.Column<bool>(type: "bit", nullable: false),
                    KichHoat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiNghiPheps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuyPhepNhanViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    TongPhepNam = table.Column<double>(type: "float", nullable: false),
                    SoPhepDaDung = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyPhepNhanViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuyPhepNhanViens_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonXinNghis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    LoaiNghiPhepId = table.Column<int>(type: "int", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNgayNghi = table.Column<double>(type: "float", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiDuyetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChuCuaNguoiDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonXinNghis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonXinNghis_LoaiNghiPheps_LoaiNghiPhepId",
                        column: x => x.LoaiNghiPhepId,
                        principalTable: "LoaiNghiPheps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonXinNghis_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonXinNghis_LoaiNghiPhepId",
                table: "DonXinNghis",
                column: "LoaiNghiPhepId");

            migrationBuilder.CreateIndex(
                name: "IX_DonXinNghis_NhanSuId",
                table: "DonXinNghis",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_QuyPhepNhanViens_NhanSuId",
                table: "QuyPhepNhanViens",
                column: "NhanSuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonXinNghis");

            migrationBuilder.DropTable(
                name: "QuyPhepNhanViens");

            migrationBuilder.DropTable(
                name: "LoaiNghiPheps");
        }
    }
}
