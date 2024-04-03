using System;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftitoFlix.Models;

namespace SoftitoFlix.Data
{
	public class SoftitoFlixContext:IdentityDbContext<ApplicationUser,ApplicationRole, long>
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
            builder.Entity<Director>(entity =>
            { 
            entity.Property(d => d.Name).IsRequired().HasColumnType("nvarchar(200)");
            entity.Property(d => d.Surname).IsRequired().HasColumnType("nvarchar(200)");
            });

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

            //UserPlan
            builder.Entity<UserPlan>(entity =>
            {
                entity.Property(up => up.StartDate).HasColumnType("date");
                entity.Property(up => up.EndDate).HasColumnType("date");
            });

            //Restriction
            builder.Entity<Restriction>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedNever();
                entity.Property(r => r.Name).IsRequired().HasColumnType("nvarchar(50)");
            });

            //Star
            builder.Entity<Star>(entity =>
            {
                entity.Property(s => s.Name).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(s => s.Surname).IsRequired().HasColumnType("nvarchar(200)");
            });



            #endregion

            #region RelationShip

            //Episode-Media
            builder.Entity<Episode>().HasOne(e => e.Media).WithMany(m => m.Episodes).HasForeignKey(e=> e.MediaId);

            //MediaCategory-Media, MediaCategory-Category

            builder.Entity<MediaCategory>().HasKey(mc => new { mc.CategoryId, mc.MediaId });
            builder.Entity<MediaCategory>().HasOne(mc => mc.Category).WithMany(c => c.MediaCategories);
            builder.Entity<MediaCategory>().HasOne(mc => mc.Media).WithMany(m => m.MediaCategories);

            //MediaDirector-Media, MediaDirector-Director

            builder.Entity<MediaDirector>().HasKey(md => new { md.DirectorId, md.MediaId });
            builder.Entity<MediaDirector>().HasOne(md => md.Media).WithMany(m => m.MediaDirectors);
            builder.Entity<MediaDirector>().HasOne(md => md.Director).WithMany(d => d.MediaDirectors);

            //MediaRestriction-Media, MediaRestriction-Restriction
            builder.Entity<MediaRestriction>().HasKey(mr => new { mr.MediaId, mr.RestrictionId });
            builder.Entity<MediaRestriction>().HasOne(mr => mr.Media).WithMany(m => m.MediaRestrictions);
            builder.Entity<MediaRestriction>().HasOne(mr => mr.Restriction).WithMany(r => r.MediaRestrictions);

            //MediaStar-Media, MediaStar-Star
            builder.Entity<MediaStar>().HasKey(md => new { md.MediaId, md.StarId });
            builder.Entity<MediaStar>().HasOne(md => md.Media).WithMany(m => m.MediaStars);
            builder.Entity<MediaStar>().HasOne(md => md.Star).WithMany(s => s.MediaStars);

            //UserFavoriteMedia-Media, UserFavoriteMedia-ApplicationUser
            builder.Entity<UserFavoriteMedia>().HasKey(um => new { um.UserId, um.MediaId });
            builder.Entity<UserFavoriteMedia>().HasOne(um => um.ApplicationUser).WithMany(au => au.UserFavoriteMedias);
            builder.Entity<UserFavoriteMedia>().HasOne(um => um.Media).WithMany();

            //UserPlan-User, UserPlan-Plan
            builder.Entity<UserPlan>().HasOne(up => up.ApplicationUser).WithMany(au => au.UserPlans).HasForeignKey(up=> up.UserId);
            builder.Entity<UserPlan>().HasOne(up => up.Plan).WithMany(p => p.UserPlans).HasForeignKey(up=> up.PlanId);

            //UserWatchEpisode-User, UserWatchEpisode-Episode
            builder.Entity<UserWatchEpisode>().HasKey(ue => new { ue.ApplicationUserId, ue.EpisodeId });
            builder.Entity<UserWatchEpisode>().HasOne(ue =>     ue.ApplicationUser).WithMany(au => au.UserWatchEpisodes);
            builder.Entity<UserWatchEpisode>().HasOne(ue => ue.Episode).WithMany(e => e.UserWatchEpisodes);
            #endregion


        }
    }
}

