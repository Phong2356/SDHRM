using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDHRM.Migrations
{
    /// <inheritdoc />
    public partial class AddThanhPhanLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThanhPhanLuongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhPhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenThanhPhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiThanhPhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhChat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KieuGiaTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KichHoat = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhPhanLuongs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThanhPhanLuongs");
        }
    }
}
