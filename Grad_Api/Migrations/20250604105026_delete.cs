using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grad_Api.Migrations
{
    /// <inheritdoc />
    public partial class delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseCategory_CourseCategoryId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizScores_AspNetUsers_StudentId",
                table: "QuizScores");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizScores_Lesson_LessonId",
                table: "QuizScores");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseCategory_CourseCategoryId",
                table: "Course",
                column: "CourseCategoryId",
                principalTable: "CourseCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScores_AspNetUsers_StudentId",
                table: "QuizScores",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScores_Lesson_LessonId",
                table: "QuizScores",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseCategory_CourseCategoryId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizScores_AspNetUsers_StudentId",
                table: "QuizScores");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizScores_Lesson_LessonId",
                table: "QuizScores");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseCategory_CourseCategoryId",
                table: "Course",
                column: "CourseCategoryId",
                principalTable: "CourseCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScores_AspNetUsers_StudentId",
                table: "QuizScores",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizScores_Lesson_LessonId",
                table: "QuizScores",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
