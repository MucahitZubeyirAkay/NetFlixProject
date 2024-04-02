using System;
using System.ComponentModel.DataAnnotations;

namespace SoftitoFlix.Models.Dtos
{
	public class EpisodeDto
	{
        [Range(0, byte.MaxValue)]
        public byte SeasonNumber { get; set; }

        [Range(0, 366)]
        public short EpisodeNumber { get; set; }

        public DateTime ReleaseDate { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = "";

        [StringLength(500)]
        public string? Description { get; set; }

        public bool Passive { get; set; }

        public TimeSpan Duration { get; set; }


        public int MediaId { get; set; }
    }
}

