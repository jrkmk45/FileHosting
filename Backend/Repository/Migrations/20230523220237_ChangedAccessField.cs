using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAccessField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPrivate",
                table: "Files");

            migrationBuilder.AddColumn<int>(
                name: "Accessability",
                table: "Files",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accessability",
                table: "Files");

            migrationBuilder.AddColumn<bool>(
                name: "isPrivate",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
