using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTableThongTinChung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongTinChungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    NoiSinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NguyenQuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhTrangHonNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaSoThue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanToc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TonGiao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuocTich = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoCCCD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCapGiayTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiCapGiayTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayHetHanGiayTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoHoChieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCapHoChieu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiCapHoChieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayHetHanHoChieu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrinhDoHocVan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrinhDoDaoTao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDaoTao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChuyenNganh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamTotNghiep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XepLoaiTotNghiep = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinChungs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongTinChungs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinChungs_NhanSuId",
                table: "ThongTinChungs",
                column: "NhanSuId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongTinChungs");
        }
    }
}
