using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Restriction
	{
		public byte Id { get; set; }

		[StringLength(50)]
		public string Name { get; set; } = "";


		public virtual List<MediaRestriction>? MediaRestrictions { get; set; }
	}
}

