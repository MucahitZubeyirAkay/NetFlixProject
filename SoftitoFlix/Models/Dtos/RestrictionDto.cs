using System.ComponentModel.DataAnnotations;

namespace SoftitoFlix.Models.Dtos
{
    public class RestrictionDto
    {
        [StringLength(50)]
        public string Name { get; set; } = "";
    }
}
