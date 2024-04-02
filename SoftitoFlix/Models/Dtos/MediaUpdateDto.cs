﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SoftitoFlix.Models.Dtos
{
	public class MediaUpdateDto
	{
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = "";

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10)]
        public float IMDBRating { get; set; }
    }
}

