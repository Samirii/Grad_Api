using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grad_Api.Data;

public partial class Course
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? TeacherName { get; set; }

    public int CourseCategoryId { get; set; }

    [ForeignKey("CourseCategoryId")]
    public virtual CourseCategory Category { get; set; } = null!;
    
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
