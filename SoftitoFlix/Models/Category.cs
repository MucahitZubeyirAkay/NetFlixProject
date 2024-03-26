using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Category
	{
		public short Id { get; set; }

		[StringLength(50, MinimumLength = 2)]
		[Column(TypeName = "nvarchar(50)")]
		public string Name { get; set; } = "";

		public List<MediaCategory>? MediaCategories { get; set; }
	}
}

