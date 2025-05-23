using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentinelTrack.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ST_Yards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Capacity = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ST_Yards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ST_Motos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Plate = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Model = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Color = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    YardId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ST_Motos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ST_Motos_ST_Yards_YardId",
                        column: x => x.YardId,
                        principalTable: "ST_Yards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ST_Motos_YardId",
                table: "ST_Motos",
                column: "YardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ST_Motos");

            migrationBuilder.DropTable(
                name: "ST_Yards");
        }
    }
}
