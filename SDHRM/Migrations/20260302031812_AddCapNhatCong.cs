using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddCapNhatCong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeNghiCapNhatCongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioVao = table.Column<TimeSpan>(type: "time", nullable: true),
                    GioRa = table.Column<TimeSpan>(type: "time", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiDuyetId = table.Column<int>(type: "int", nullable: true),
                    GhiChuCuaNguoiDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeNghiCapNhatCongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeNghiCapNhatCongs_NhanSus_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "NhanSus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeNghiCapNhatCongs_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeNghiCapNhatCongs_NguoiDuyetId",
                table: "DeNghiCapNhatCongs",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeNghiCapNhatCongs_NhanSuId",
                table: "DeNghiCapNhatCongs",
                column: "NhanSuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeNghiCapNhatCongs");
        }
    }
}
