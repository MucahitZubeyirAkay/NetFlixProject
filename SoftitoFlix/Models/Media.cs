using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Media
	{

        public int Id { get; set; }

		[StringLength(200, MinimumLength =1)]
		public string Name { get; set; } = "";

		[StringLength(500)]
		public string? Description { get; set; }

        public bool Passive { get; set; }

        [Range(0, 10)]
		public float IMDBRating { get; set; }

		public List<MediaCategory>? MediaCategories { get; set; }
        public List<MediaRestriction>? Restrictions { get; set; }
        public List<MediaDirector>? MediaDirectors { get; set; }
		public List<MediaStar>? MediaStars { get; set; }

    }
}

