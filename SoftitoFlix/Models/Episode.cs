using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Episode
	{

		public long Id { get; set; }

		[Range(0, byte.MaxValue)]
		public byte SeasonNumber { get; set; }

		[Range(0, 366)]
		public short EpisodeNumber { get; set; }

		public DateTime ReleaseDate { get; set; }

		[StringLength(200, MinimumLength =1)]	
		[Column(TypeName = "nvarchar(200)")]
		public string Title { get; set; } = "";

        [StringLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? Description { get; set; }

		public long ViewCount { get; set; }

		public bool Passive { get; set; }

		public TimeSpan Duration { get; set; }


        [ForeignKey(nameof(MediaId))]
        public int MediaId { get; set; }



        public Media? Media { get; set; }

	}
}

