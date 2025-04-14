using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grad_Api.Migrations
{
    /// <inheritdoc />
    public partial class removeThub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Course");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Course",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
