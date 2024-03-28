using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Person
	{
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = "";
    }
}

