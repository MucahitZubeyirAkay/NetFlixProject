using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Category
	{
		public short Id { get; set; }

		public string Name { get; set; } = "";

		public virtual List<MediaCategory>? MediaCategories { get; set; }
	}
}

