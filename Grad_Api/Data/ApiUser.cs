using Grad_Api.Migrations;
using Microsoft.AspNetCore.Identity;

namespace Grad_Api.Data
{
    public class ApiUser : IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
    }}
