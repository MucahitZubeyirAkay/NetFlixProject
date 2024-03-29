using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SoftitoFlix.Models
{
	public class ApplicationUser:IdentityUser<long>
	{

        public DateTime BirthDate { get; set; }

        [StringLength(100, MinimumLength =2)]
        public string FullName { get; set; } = "";

        public bool Passive { get; set; }

        public DateTime RegisterDate { get; set; }


        public virtual List<UserFavoriteMedia>? UserFavoriteMedias { get; set; }

        public virtual List<UserPlan>? UserPlans { get; set; }

        public virtual List<UserWatchEpisode>? UserWatchEpisodes { get; set; }
    }
}

