using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Menulo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchScopeToMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchScope",
                table: "tbl_menu_items",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "ThisBranchOnly");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchScope",
                table: "tbl_menu_items");
        }
    }
}
