using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Restriction
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte Id { get; set; }

		[Required]
		[StringLength(50)]
		[Column(TypeName = "nvarchar(50)")]
		public string Name { get; set; } = "";

		public List<MediaRestriction>? MediaRestrictions { get; set; }
	}
}

