using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addImageAndFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageBase64",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextFileBase64",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBase64",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TextFileBase64",
                table: "Comments");
        }
    }
}
