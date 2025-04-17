using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grad_Api.Data;

public partial class Lesson
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? VideoUrl { get; set; }

    public string? Content { get; set; }

    [ForeignKey("Course")]
    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;
}
