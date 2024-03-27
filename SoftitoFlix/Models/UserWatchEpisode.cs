using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class UserWatchEpisode
	{
        [ForeignKey(nameof(EpisodeId))]
        public int EpisodeId { get; set; }

        [ForeignKey(nameof(UserId))]
        public long UserId { get; set; }


        public Episode? Episode { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
    }
}

