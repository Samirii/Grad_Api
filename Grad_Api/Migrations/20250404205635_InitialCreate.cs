using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grad_Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(name: "Name ", type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CourseCa__3214EC07415C3794", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(name: "Title ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(name: "Description ", type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ThumbnailUrl = table.Column<string>(name: "ThumbnailUrl ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<int>(name: "CategoryId ", type: "int", nullable: true),
                    InstructorId = table.Column<int>(name: "InstructorId ", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Course__3214EC073AE4FFCA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_CourseCategory_CategoryId ",
                        column: x => x.CategoryId,
                        principalTable: "CourseCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(name: "Title ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VideoUrl = table.Column<string>(name: "VideoUrl ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(name: "Content ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lesson__3214EC07751A4E00", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lesson_ToTable",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(name: "Title ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Quiz__3214EC0747465A37", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiz_ToTable",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(name: "QuestionText ", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuizId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__3214EC079DB2C3F8", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_ToTable",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CategoryId ",
                table: "Course",
                column: "CategoryId ");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId",
                table: "Lesson",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuizId",
                table: "Question",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_CourseId",
                table: "Quiz",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lesson");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "CourseCategory");
        }
    }
}
