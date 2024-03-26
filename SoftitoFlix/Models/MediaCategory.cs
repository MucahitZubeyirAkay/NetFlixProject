using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaCategory
	{
		[ForeignKey(nameof(MediaId))]
		public int MediaId { get; set; }

		[ForeignKey(nameof(CategoryId))]
		public short CategoryId { get; set; }



		public Media? Media { get; set; }

		public Category? Category { get; set; }
	}
}

