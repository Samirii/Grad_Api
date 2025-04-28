namespace Grad_Api.Models.User
{
    public class UserReadDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
