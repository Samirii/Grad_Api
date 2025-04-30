using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.Quiz
{
    public class QuizCreateDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }


    }
}
