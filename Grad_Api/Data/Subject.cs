using System.Collections.Generic;

namespace Grad_Api.Data
{
    public class Subject
    {
        public Subject()
        {
            Teachers = new HashSet<ApiUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApiUser> Teachers { get; set; }
    }
}
