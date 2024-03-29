﻿using System;
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

		public virtual List<Episode>? Episodes { get; set; }
		public virtual List<MediaCategory>? MediaCategories { get; set; }
        public virtual List<MediaRestriction>? MediaRestrictions { get; set; }
        public virtual List<MediaDirector>? MediaDirectors { get; set; }
		public virtual List<MediaStar>? MediaStars { get; set; }

    }
}

