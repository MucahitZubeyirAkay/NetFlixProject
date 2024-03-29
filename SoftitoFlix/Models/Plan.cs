using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Plan
	{
		public short Id { get; set; }

		[StringLength(50, MinimumLength =2)]
		public string Name { get; set; } = "";

		[Range(0, float.MaxValue)]
		public float Price { get; set; }

        [StringLength(20, MinimumLength = 2)]
        public string? Resolution { get; set; }


		public virtual List<UserPlan>? UserPlans { get; set; }
	}
}

