using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaDirector
	{
		[ForeignKey(nameof(MediaId))]
		public int MediaId { get; set; }

		[ForeignKey(nameof(DirectorId))]
		public int DirectorId { get; set; }



		public Media? Media { get; set; }

		public Director? Director { get; set; }
	}
}

