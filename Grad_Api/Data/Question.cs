using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Grad_Api.Data;

public partial class Question
{
    public int Id { get; set; }

    public string? QuestionText { get; set; }

    public string? OptionA { get; set; }

    public string? OptionB { get; set; }

    public string? OptionC { get; set; }

    public string? OptionD { get; set; }

    public string? CorrectAnswer { get; set; }
    public string Defficulty { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string CourseCategory { get; set; } = string.Empty;
    public int UnitNumber { get; set; }
    

    public int? QuizId { get; set; }
    [JsonIgnore]
    public virtual Quiz? Quiz { get; set; }
    
}
