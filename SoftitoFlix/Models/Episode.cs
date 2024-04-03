using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftitoFlix.Models
{
	public class Episode
	{

		public long Id { get; set; }

		public byte SeasonNumber { get; set; }

		public short EpisodeNumber { get; set; }

		public DateTime ReleaseDate { get; set; }

		public string Title { get; set; } = "";

        public string? Description { get; set; }

		public long ViewCount { get; set; }

		public bool Passive { get; set; }

		public TimeSpan Duration { get; set; }


        public int MediaId { get; set; }

        public Media? Media { get; set; }

		public virtual List<UserWatchEpisode>? UserWatchEpisodes { get; set; }

	}
}

