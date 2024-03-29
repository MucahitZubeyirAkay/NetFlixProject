using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class UserPlan
	{
		public long Id { get; set; }

		public long UserId { get; set; }

        public short PlanId { get; set; }


		public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }



		public Plan? Plan { get; set; }

		public ApplicationUser? ApplicationUser { get; set; }
	}
}

