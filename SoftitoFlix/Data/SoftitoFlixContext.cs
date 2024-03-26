using System;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Models;

namespace SoftitoFlix.Data
{
	public class SoftitoFlixContext:DbContext
	{
		public SoftitoFlixContext(DbContextOptions<SoftitoFlixContext> options): base(options)
		{
			
		}

		public DbSet<Category>? Categories { get; set; } = default;
    }
}

