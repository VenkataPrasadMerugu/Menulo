using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Menulo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServesToMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchScope",
                table: "tbl_menu_items");

            migrationBuilder.AddColumn<int>(
                name: "Serves",
                table: "tbl_menu_items",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Serves",
                table: "tbl_menu_items");

            migrationBuilder.AddColumn<string>(
                name: "BranchScope",
                table: "tbl_menu_items",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");
        }
    }
}
