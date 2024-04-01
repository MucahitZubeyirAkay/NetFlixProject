using System.ComponentModel.DataAnnotations;

namespace SoftitoFlix.Models.Dtos
{
    public class StarDto
    {
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = "";

        [StringLength(200, MinimumLength = 1)]
        public string Surname { get; set; } = "";
    }
}
