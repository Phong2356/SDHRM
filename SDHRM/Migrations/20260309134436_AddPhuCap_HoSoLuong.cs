using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddPhuCap_HoSoLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PhuCapChucVu",
                table: "HoSoLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhuCapDiLai",
                table: "HoSoLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhuCapKhac",
                table: "HoSoLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhuCapChucVu",
                table: "HoSoLuongs");

            migrationBuilder.DropColumn(
                name: "PhuCapDiLai",
                table: "HoSoLuongs");

            migrationBuilder.DropColumn(
                name: "PhuCapKhac",
                table: "HoSoLuongs");
        }
    }
}
