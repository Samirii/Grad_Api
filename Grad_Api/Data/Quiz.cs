﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Data;

public partial class Quiz
{
    public int Id { get; set; }

    public string? Title { get; set; }
    public int LessonId { get; set; }

    public virtual Lesson Lesson { get; set; }
 

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

}
