using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grad_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectToApiUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Subject",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "AspNetUsers");
        }
    }
}
