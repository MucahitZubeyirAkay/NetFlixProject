using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class UserFavoriteMedia
	{
		public int MediaId { get; set; }

		public long UserId { get; set; }


		public Media? Media { get; set; }

		public ApplicationUser? ApplicationUser { get; set; }
	}
}

