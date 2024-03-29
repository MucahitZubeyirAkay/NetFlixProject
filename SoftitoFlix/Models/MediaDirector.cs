using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaDirector
	{
		public int MediaId { get; set; }
	
		public int DirectorId { get; set; }



		public Media? Media { get; set; }

		public Director? Director { get; set; }
	}
}

