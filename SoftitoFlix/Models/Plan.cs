using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class Plan
	{
		public short Id { get; set; }

		public string Name { get; set; } = "";

		public float Price { get; set; }

        public string? Resolution { get; set; }

		public bool Passive { get; set; }


		public virtual List<UserPlan>? UserPlans { get; set; }
	}
}

