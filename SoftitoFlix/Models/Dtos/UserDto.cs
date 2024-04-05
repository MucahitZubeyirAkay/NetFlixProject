using System.ComponentModel.DataAnnotations;

namespace SoftitoFlix.Models.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";
        public DateTime BirthDate { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = "";

        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        public  string Email { get; set; } = "";

        [Phone]
        [StringLength(30)]
        public  string? PhoneNumber { get; set; }

    }
}
