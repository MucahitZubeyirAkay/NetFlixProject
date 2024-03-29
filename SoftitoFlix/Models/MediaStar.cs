using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaStar
	{
		public int StarId { get; set; }

		public int MediaId { get; set; }




		public Star? Star { get; set; }

		public Media? Media { get; set; }

	}
}

