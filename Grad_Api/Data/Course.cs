using System;
using System.Collections.Generic;

namespace Grad_Api.Data;

public partial class Course
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }


    public string? TeacherName { get; set; }

    public int? CategoryId { get; set; }
    public CourseCategory Category { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
