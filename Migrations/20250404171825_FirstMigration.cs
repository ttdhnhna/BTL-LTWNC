using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTL_LTWNC.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Post",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaThue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayThue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoCho = table.Column<int>(type: "int", nullable: false),
                    LoaiNL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaDiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhNang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MTOther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    imgURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Post", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Vehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sCarName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sCarNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fGiaThue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dNgayThue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dNgayTra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    sSoCho = table.Column<int>(type: "int", nullable: false),
                    sLoaiNL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sDiaDiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sTinhNang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sMoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sMTOther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imgURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Vehicle", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Post");

            migrationBuilder.DropTable(
                name: "tbl_Vehicle");
        }
    }
}
