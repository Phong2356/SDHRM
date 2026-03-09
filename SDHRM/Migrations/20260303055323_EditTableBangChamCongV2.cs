using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class EditTableBangChamCongV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TinhNhanVienKiemNhiem",
                table: "BangChamCongs");

            migrationBuilder.AddColumn<string>(
                name: "CachTinhCong",
                table: "BangChamCongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachTinhCong",
                table: "BangChamCongs");

            migrationBuilder.AddColumn<bool>(
                name: "TinhNhanVienKiemNhiem",
                table: "BangChamCongs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
