using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class UserWatchEpisode
	{
        public long EpisodeId { get; set; }

        public long ApplicationUserId { get; set; }


        public Episode? Episode { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
    }
}

