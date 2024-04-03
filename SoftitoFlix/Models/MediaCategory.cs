using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftitoFlix.Models
{
	public class MediaCategory
	{

		public int MediaId { get; set; }

		public short CategoryId { get; set; }


		public Media? Media { get; set; }

		public Category? Category { get; set; }
	}
}

