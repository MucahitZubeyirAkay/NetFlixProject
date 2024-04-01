using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SoftitoFlix.Models
{
	public class Director:Person
	{

		public virtual List<MediaDirector>? MediaDirectors { get; set; }

    }
}

