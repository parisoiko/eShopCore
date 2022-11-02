using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopCore.Migrations
{
    public partial class newMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Products");

            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "Products");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Products",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
