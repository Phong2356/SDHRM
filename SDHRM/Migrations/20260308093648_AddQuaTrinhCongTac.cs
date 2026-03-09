using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddQuaTrinhCongTac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuaTrinhCongTacs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhongBanId = table.Column<int>(type: "int", nullable: false),
                    ViTriCongViecId = table.Column<int>(type: "int", nullable: false),
                    TinhChatLaoDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuanLyTrucTiepId = table.Column<int>(type: "int", nullable: true),
                    SoQuyetDinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayQuyetDinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TepDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuaTrinhCongTacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuaTrinhCongTacs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuaTrinhCongTacs_NhanSus_QuanLyTrucTiepId",
                        column: x => x.QuanLyTrucTiepId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuaTrinhCongTacs_PhongBans_PhongBanId",
                        column: x => x.PhongBanId,
                        principalTable: "PhongBans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuaTrinhCongTacs_ViTriCongViecs_ViTriCongViecId",
                        column: x => x.ViTriCongViecId,
                        principalTable: "ViTriCongViecs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhCongTacs_NhanSuId",
                table: "QuaTrinhCongTacs",
                column: "NhanSuId");

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhCongTacs_PhongBanId",
                table: "QuaTrinhCongTacs",
                column: "PhongBanId");

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhCongTacs_QuanLyTrucTiepId",
                table: "QuaTrinhCongTacs",
                column: "QuanLyTrucTiepId");

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhCongTacs_ViTriCongViecId",
                table: "QuaTrinhCongTacs",
                column: "ViTriCongViecId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuaTrinhCongTacs");
        }
    }
}
