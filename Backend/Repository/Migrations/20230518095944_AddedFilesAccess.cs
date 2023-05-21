using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddedFilesAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPrivate",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FileMetadataUser",
                columns: table => new
                {
                    AccessedFilesId = table.Column<string>(type: "text", nullable: false),
                    PermittedUsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadataUser", x => new { x.AccessedFilesId, x.PermittedUsersId });
                    table.ForeignKey(
                        name: "FK_FileMetadataUser_AspNetUsers_PermittedUsersId",
                        column: x => x.PermittedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileMetadataUser_Files_AccessedFilesId",
                        column: x => x.AccessedFilesId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadataUser_PermittedUsersId",
                table: "FileMetadataUser",
                column: "PermittedUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMetadataUser");

            migrationBuilder.DropColumn(
                name: "isPrivate",
                table: "Files");
        }
    }
}
