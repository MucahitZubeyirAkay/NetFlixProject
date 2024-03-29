using System;
using System.ComponentModel.DataAnnotations;

namespace SoftitoFlix.Models.Dtos
{
	public class CategoryDto
	{
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = "";
    }
}

