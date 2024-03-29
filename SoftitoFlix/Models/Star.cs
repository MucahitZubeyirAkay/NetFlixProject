using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Star : Person
	{
		public virtual List<MediaStar>? MediaStars { get; set; }
	}
}

