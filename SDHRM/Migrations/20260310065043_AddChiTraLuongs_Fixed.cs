using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddChiTraLuongs_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiTietPhieuChiLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuChiLuongId = table.Column<int>(type: "int", nullable: false),
                    ChiTietBangLuongId = table.Column<int>(type: "int", nullable: false),
                    SoTienChi = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuChiLuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuChiLuongs_ChiTietBangLuongs_ChiTietBangLuongId",
                        column: x => x.ChiTietBangLuongId,
                        principalTable: "ChiTietBangLuongs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuChiLuongs_PhieuChiLuongs_PhieuChiLuongId",
                        column: x => x.PhieuChiLuongId,
                        principalTable: "PhieuChiLuongs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuChiLuongs_ChiTietBangLuongId",
                table: "ChiTietPhieuChiLuongs",
                column: "ChiTietBangLuongId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuChiLuongs_PhieuChiLuongId",
                table: "ChiTietPhieuChiLuongs",
                column: "PhieuChiLuongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietPhieuChiLuongs");
        }
    }
}
