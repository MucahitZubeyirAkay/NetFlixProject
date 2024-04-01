using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Restriction
	{
		public byte Id { get; set; }

		public string Name { get; set; } = "";

        public virtual List<MediaRestriction>? MediaRestrictions { get; set; }
	}
}

