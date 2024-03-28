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


            #region ModelConfiguration

            //ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(au => au.UserName).IsUnique(true);
                entity.Property(au => au.FullName).IsRequired().HasColumnType("nvarchar(100)");
                entity.Property(au => au.BirthDate).IsRequired().HasColumnType("date");
                entity.Property(au => au.Email).IsRequired().HasColumnType("varchar(50)");
                entity.Property(au => au.PhoneNumber).IsRequired().HasColumnType("varchar(30)");

            });

            //Category
            builder.Entity<Category>().Property(c => c.Name).IsRequired().HasColumnType("nvarchar(50)");

            //Director
            builder.Entity<Director>().Property(d => d.Name).IsRequired().HasColumnType("nvarchar(200)");

            //Episode
            builder.Entity<Episode>(entity =>
            {
                entity.HasIndex(e => new { e.MediaId, e.SeasonNumber, e.EpisodeNumber });
                entity.Property(e => e.Description).HasColumnType("nvarchar(500)");
                entity.Property(e => e.ReleaseDate).IsRequired().HasColumnType("date");
                entity.Property(e => e.Title).IsRequired().HasColumnType("nvarchar(500)");
            });

            //Media
            builder.Entity<Media>(entity =>
            {
                entity.Property(m => m.Name).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(e => e.Description).HasColumnType("nvarchar(500)");
            });

            //Plan
            builder.Entity<Plan>(entity =>
            {
                entity.Property(p => p.Name).IsRequired().HasColumnType("nvarchar(50)");
                entity.Property(p => p.Price).IsRequired();
                entity.Property(p => p.Resolution).HasColumnType("varchar(20)");
            });

            //Restriction


            #endregion



            builder.Entity<MediaCategory>().HasKey(mc => new { mc.CategoryId, mc.MediaId });
            builder.Entity<MediaDirector>().HasKey(mc => new { mc.DirectorId, mc.MediaId });
            builder.Entity<MediaRestriction>().HasKey(mc => new { mc.RestrictionId, mc.MediaId });
            builder.Entity<MediaStar>().HasKey(mc => new { mc.StarId, mc.MediaId });

            builder.Entity<UserFavoriteMedia>().HasKey(um => new { um.UserId, um.MediaId });
            builder.Entity<UserWatchEpisode>().HasKey(ue => new { ue.UserId, ue.EpisodeId });
        }
    }
}

