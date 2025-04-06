using System;
using System.Collections.Generic;

namespace Grad_Api.Data;

public partial class CourseCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public ICollection<Course> Courses { get; set; }

}
