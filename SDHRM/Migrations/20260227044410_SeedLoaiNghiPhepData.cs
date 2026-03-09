using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class SeedLoaiNghiPhepData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LoaiNghiPheps",
                columns: new[] { "Id", "CoHuongLuong", "KichHoat", "TenLoai", "TruVaoQuyPhep" },
                values: new object[,]
                {
                    { 1, true, true, "Nghỉ phép năm", true },
                    { 2, false, true, "Nghỉ không lương", false },
                    { 3, false, true, "Nghỉ ốm hưởng BHXH", false },
                    { 4, false, true, "Nghỉ thai sản", false },
                    { 5, true, true, "Nghỉ bù", false },
                    { 6, true, true, "Nghỉ kết hôn/ma chay", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LoaiNghiPheps",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
