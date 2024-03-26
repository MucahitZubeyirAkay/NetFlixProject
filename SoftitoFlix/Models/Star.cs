using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Star : Person
	{
		public List<MediaStar>? MediaStars { get; set; }
	}
}

