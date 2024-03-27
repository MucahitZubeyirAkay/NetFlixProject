using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class UserPlan
	{
		public long Id { get; set; }

		[ForeignKey(nameof(UserId))]
		public int UserId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public int PlanId { get; set; }

		[Column(TypeName ="date")]
		public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

		public Plan? Plan { get; set; }

		public ApplicationUser? ApplicationUser { get; set; }
	}
}

