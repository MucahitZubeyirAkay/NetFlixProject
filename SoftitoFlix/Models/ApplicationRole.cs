using Microsoft.AspNetCore.Identity;

namespace SoftitoFlix.Models
{
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
        public ApplicationRole()
        {

        }
    }
}
