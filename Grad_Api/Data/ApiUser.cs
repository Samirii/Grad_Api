using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Data
{
    public class ApiUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public string? CvFilePath { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<QuizScore> QuizScores { get; set; } = new List<QuizScore>();



    }


}
