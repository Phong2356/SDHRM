using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class FixPhongBanRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NhanSus_PhongBanId",
                table: "NhanSus");

            migrationBuilder.CreateIndex(
                name: "IX_PhongBans_IDTruongPhong",
                table: "PhongBans",
                column: "IDTruongPhong");

            migrationBuilder.CreateIndex(
                name: "IX_NhanSus_PhongBanId",
                table: "NhanSus",
                column: "PhongBanId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhongBans_NhanSus_IDTruongPhong",
                table: "PhongBans",
                column: "IDTruongPhong",
                principalTable: "NhanSus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhongBans_NhanSus_IDTruongPhong",
                table: "PhongBans");

            migrationBuilder.DropIndex(
                name: "IX_PhongBans_IDTruongPhong",
                table: "PhongBans");

            migrationBuilder.DropIndex(
                name: "IX_NhanSus_PhongBanId",
                table: "NhanSus");

            migrationBuilder.CreateIndex(
                name: "IX_NhanSus_PhongBanId",
                table: "NhanSus",
                column: "PhongBanId",
                unique: true);
        }
    }
}
