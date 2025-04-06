using System;
using System.Collections.Generic;

namespace Grad_Api.Data;

public partial class Quiz
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? CourseId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
