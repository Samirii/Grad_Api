using System;
using System.Collections.Generic;

namespace Grad_Api.Data;

public partial class Question
{
    public int Id { get; set; }

    public string? QuestionText { get; set; }

    public int? QuizId { get; set; }

    public virtual Quiz? Quiz { get; set; }
}
