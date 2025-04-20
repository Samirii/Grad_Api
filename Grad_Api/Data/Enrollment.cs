namespace Grad_Api.Data
{
    public class Enrollment
    {
        public int Id { get; set; }

        public string StudentId { get; set; }
        public int CourseId { get; set; }

        public ApiUser Student { get; set; }
        public Course Course { get; set; }
    }
}
