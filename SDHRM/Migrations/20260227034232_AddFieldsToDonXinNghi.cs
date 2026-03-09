using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToDonXinNghi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NguoiDuyetId",
                table: "DonXinNghis",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileDinhKem",
                table: "DonXinNghis",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonXinNghis_NguoiDuyetId",
                table: "DonXinNghis",
                column: "NguoiDuyetId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonXinNghis_NhanSus_NguoiDuyetId",
                table: "DonXinNghis",
                column: "NguoiDuyetId",
                principalTable: "NhanSus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonXinNghis_NhanSus_NguoiDuyetId",
                table: "DonXinNghis");

            migrationBuilder.DropIndex(
                name: "IX_DonXinNghis_NguoiDuyetId",
                table: "DonXinNghis");

            migrationBuilder.DropColumn(
                name: "FileDinhKem",
                table: "DonXinNghis");

            migrationBuilder.AlterColumn<string>(
                name: "NguoiDuyetId",
                table: "DonXinNghis",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
