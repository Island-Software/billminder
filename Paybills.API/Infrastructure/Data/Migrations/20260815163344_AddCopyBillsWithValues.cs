using Microsoft.EntityFrameworkCore.Migrations;

namespace Paybills.API.Data.Migrations
{
    public partial class AddCopyBillsWithValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CopyBillsValues",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CopyBillsValues",
                table: "Users");
        }
    }
}
