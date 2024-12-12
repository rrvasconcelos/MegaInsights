using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MI.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LotteryResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrawDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    Accumulated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Result01 = table.Column<int>(type: "int", nullable: false),
                    Result02 = table.Column<int>(type: "int", nullable: false),
                    Result03 = table.Column<int>(type: "int", nullable: false),
                    Result04 = table.Column<int>(type: "int", nullable: false),
                    Result05 = table.Column<int>(type: "int", nullable: false),
                    Result06 = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotteryResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotteryResults");
        }
    }
}
