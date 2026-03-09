using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddDonTangCa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonTangCas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    NgayTangCa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TuGio = table.Column<TimeSpan>(type: "time", nullable: false),
                    DenGio = table.Column<TimeSpan>(type: "time", nullable: false),
                    SoGio = table.Column<double>(type: "float", nullable: false),
                    LyDoTangCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiDuyetId = table.Column<int>(type: "int", nullable: true),
                    GhiChuCuaNguoiDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonTangCas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonTangCas_NhanSus_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "NhanSus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DonTangCas_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonTangCas_NguoiDuyetId",
                table: "DonTangCas",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_DonTangCas_NhanSuId",
                table: "DonTangCas",
                column: "NhanSuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonTangCas");
        }
    }
}
