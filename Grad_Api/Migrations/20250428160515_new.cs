using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grad_Api.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizScore_AspNetUsers_StudentId",
                table: "QuizScore");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizScore_Quiz_QuizId",
                table: "QuizScore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizScore",
                table: "QuizScore");

            migrationBuilder.RenameTable(
                name: "QuizScore",
                newName: "QuizScores");

            migrationBuilder.RenameIndex(
                name: "IX_QuizScore_StudentId",
                table: "QuizScores",
                newName: "IX_QuizScores_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizScore_QuizId",
                table: "QuizScores",
                newName: "IX_QuizScores_QuizId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizScores",
                table: "QuizScores",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScores_AspNetUsers_StudentId",
                table: "QuizScores",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScores_Quiz_QuizId",
                table: "QuizScores",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizScores_AspNetUsers_StudentId",
                table: "QuizScores");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizScores_Quiz_QuizId",
                table: "QuizScores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizScores",
                table: "QuizScores");

            migrationBuilder.RenameTable(
                name: "QuizScores",
                newName: "QuizScore");

            migrationBuilder.RenameIndex(
                name: "IX_QuizScores_StudentId",
                table: "QuizScore",
                newName: "IX_QuizScore_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizScores_QuizId",
                table: "QuizScore",
                newName: "IX_QuizScore_QuizId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizScore",
                table: "QuizScore",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScore_AspNetUsers_StudentId",
                table: "QuizScore",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScore_Quiz_QuizId",
                table: "QuizScore",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
