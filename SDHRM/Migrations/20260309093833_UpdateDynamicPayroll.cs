using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDynamicPayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LuongOT",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "LuongThucTe",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "PhatDiMuon",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "PhụCap",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "TruBHTN",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "TruBHXH",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "TruBHYT",
                table: "ChiTietBangLuongs");

            migrationBuilder.DropColumn(
                name: "TruThueTNCN",
                table: "ChiTietBangLuongs");

            migrationBuilder.AddColumn<int>(
                name: "MauBangLuongId",
                table: "BangLuongs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KetQuaLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChiTietBangLuongId = table.Column<int>(type: "int", nullable: false),
                    ThanhPhanLuongId = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaLuongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KetQuaLuongs_ChiTietBangLuongs_ChiTietBangLuongId",
                        column: x => x.ChiTietBangLuongId,
                        principalTable: "ChiTietBangLuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQuaLuongs_ThanhPhanLuongs_ThanhPhanLuongId",
                        column: x => x.ThanhPhanLuongId,
                        principalTable: "ThanhPhanLuongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangLuongs_MauBangLuongId",
                table: "BangLuongs",
                column: "MauBangLuongId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaLuongs_ChiTietBangLuongId",
                table: "KetQuaLuongs",
                column: "ChiTietBangLuongId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaLuongs_ThanhPhanLuongId",
                table: "KetQuaLuongs",
                column: "ThanhPhanLuongId");

            migrationBuilder.AddForeignKey(
                name: "FK_BangLuongs_MauBangLuongs_MauBangLuongId",
                table: "BangLuongs",
                column: "MauBangLuongId",
                principalTable: "MauBangLuongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BangLuongs_MauBangLuongs_MauBangLuongId",
                table: "BangLuongs");

            migrationBuilder.DropTable(
                name: "KetQuaLuongs");

            migrationBuilder.DropIndex(
                name: "IX_BangLuongs_MauBangLuongId",
                table: "BangLuongs");

            migrationBuilder.DropColumn(
                name: "MauBangLuongId",
                table: "BangLuongs");

            migrationBuilder.AddColumn<decimal>(
                name: "LuongOT",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LuongThucTe",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhatDiMuon",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PhụCap",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TruBHTN",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TruBHXH",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TruBHYT",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TruThueTNCN",
                table: "ChiTietBangLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
