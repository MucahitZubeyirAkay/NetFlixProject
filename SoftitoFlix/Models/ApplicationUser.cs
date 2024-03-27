using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SoftitoFlix.Models
{
	public class ApplicationUser:IdentityUser<long>
	{
        [Column(TypeName ="date")]
        public DateTime BirthDate { get; set; }

        [StringLength(100, MinimumLength =2)]
        [Column(TypeName ="nvarchar(100)")]
        public string Name { get; set; } = "";

        public bool Passive { get; set; }

    }
}

