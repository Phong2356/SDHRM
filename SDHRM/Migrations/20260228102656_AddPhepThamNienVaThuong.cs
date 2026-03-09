using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddPhepThamNienVaThuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PhepThamNien",
                table: "QuyPhepNhanViens",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PhepThuong",
                table: "QuyPhepNhanViens",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PhepTonNamTruoc",
                table: "QuyPhepNhanViens",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhepThamNien",
                table: "QuyPhepNhanViens");

            migrationBuilder.DropColumn(
                name: "PhepThuong",
                table: "QuyPhepNhanViens");

            migrationBuilder.DropColumn(
                name: "PhepTonNamTruoc",
                table: "QuyPhepNhanViens");
        }
    }
}
