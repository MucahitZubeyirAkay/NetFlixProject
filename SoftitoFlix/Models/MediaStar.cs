using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaStar
	{
		[ForeignKey(nameof(StarId))]
		public int StarId { get; set; }

		[ForeignKey(nameof(MediaId))]
		public int MediaId { get; set; }




		public Star? Star { get; set; }

		public Media? Media { get; set; }

	}
}

