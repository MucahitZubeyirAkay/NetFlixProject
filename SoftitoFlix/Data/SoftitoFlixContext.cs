using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Models;

namespace SoftitoFlix.Data
{
	public class SoftitoFlixContext:IdentityDbContext<ApplicationUser, ApplicationRole, long>
	{
		public SoftitoFlixContext(DbContextOptions<SoftitoFlixContext> options): base(options)
		{
			
		}

		public DbSet<Category> Categories { get; set; } = default!;

        public DbSet<Director> Directors { get; set; } = default!;

        public DbSet<Episode> Episodes { get; set; } = default!;

        public DbSet<Media> Medias { get; set; } = default!;

        public DbSet<MediaCategory> MediaCategories { get; set; } = default!;

        public DbSet<MediaDirector> MediaDirectors { get; set; } = default!;

        public DbSet<MediaRestriction> MediaRestrictions { get; set; } = default!;

        public DbSet<MediaStar> MediaStars { get; set; } = default!;

        public DbSet<Plan> Plans { get; set; } = default!;

        public DbSet<Restriction> Restrictions { get; set; } = default!;

        public DbSet<Star> Stars { get; set; } = default!;

        public DbSet<UserFavoriteMedia> UsersFavoriteMedias { get; set; } = default!;

        public DbSet<UserPlan> UserPlans { get; set; } = default!;

        public DbSet<UserWatchEpisode> UsersWatchEpisodes { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MediaCategory>().HasKey(mc => new { mc.CategoryId, mc.MediaId });
            builder.Entity<MediaDirector>().HasKey(mc => new { mc.DirectorId, mc.MediaId });
            builder.Entity<MediaRestriction>().HasKey(mc => new { mc.RestrictionId, mc.MediaId });
            builder.Entity<MediaStar>().HasKey(mc => new { mc.StarId, mc.MediaId });

            builder.Entity<UserFavoriteMedia>().HasKey(um => new { um.UserId, um.MediaId });
            builder.Entity<UserPlan>().HasKey(up => new { up.UserId, up.PlanId });
            builder.Entity<UserWatchEpisode>().HasKey(ue => new { ue.UserId, ue.EpisodeId });
        }
    }
}

