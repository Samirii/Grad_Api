using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grad_Api.Data;

public partial class CourseCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
