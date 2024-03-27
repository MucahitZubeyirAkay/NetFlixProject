using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class UserFavoriteMedia
	{
		[ForeignKey(nameof(MediaId))]
		public int MediaId { get; set; }

		[ForeignKey(nameof(UserId))]
		public long UserId { get; set; }


		public Media? Media { get; set; }

		public ApplicationUser? ApplicationUser { get; set; }
	}
}

