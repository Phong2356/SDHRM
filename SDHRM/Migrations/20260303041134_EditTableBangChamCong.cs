using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class EditTableBangChamCong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhThucChamCong",
                table: "BangChamCongs");

            migrationBuilder.DropColumn(
                name: "LoaiCongChuan",
                table: "BangChamCongs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhThucChamCong",
                table: "BangChamCongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoaiCongChuan",
                table: "BangChamCongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
