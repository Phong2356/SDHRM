using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddTableThongTinLienHe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongTinLienHes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhanSuId = table.Column<int>(type: "int", nullable: false),
                    DTCoQuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTNhaRieng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTKhac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCoQuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailKhac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkMangXaHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiThuongTru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiTamTru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HTLienHeKhanCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTLienHeKhanCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailLienHeKhanCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiLienHeKhanCap = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinLienHes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongTinLienHes_NhanSus_NhanSuId",
                        column: x => x.NhanSuId,
                        principalTable: "NhanSus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongTinLienHes_NhanSuId",
                table: "ThongTinLienHes",
                column: "NhanSuId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongTinLienHes");
        }
    }
}
