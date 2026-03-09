using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddDiMuonVeSom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonDiMuonVeSoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    LoaiDangKy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoPhut = table.Column<int>(type: "int", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiDuyetId = table.Column<int>(type: "int", nullable: true),
                    GhiChuCuaNguoiDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonDiMuonVeSoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonDiMuonVeSoms_NhanSus_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "NhanSus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DonDiMuonVeSoms_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonDiMuonVeSoms_NguoiDuyetId",
                table: "DonDiMuonVeSoms",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DonDiMuonVeSoms_NhanSuId",
                table: "DonDiMuonVeSoms",
                column: "NhanSuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonDiMuonVeSoms");
        }
    }
}
