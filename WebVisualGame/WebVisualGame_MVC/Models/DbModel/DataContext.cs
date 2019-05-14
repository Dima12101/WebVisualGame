using Microsoft.EntityFrameworkCore;
using System;

namespace WebVisualGame_MVC.Data
{
	public class DataContext : DbContext
	{
		public DbSet<User> Users { get; set; }


		public DbSet<Game> Games { get; set; }

		public DbSet<GameComponents.SavedGame> SavedGames { get; set; }


		public DbSet<GameComponents.Image> Images { get; set; }

		public DbSet<GameComponents.PointDialog> PointDialogs { get; set; }

		public DbSet<GameComponents.Transition> Transitions { get; set; }

		public DbSet<GameComponents.TransitionAction> TransitionActions { get; set; }


		public DbSet<GameComponents.Review> Reviews { get; set; }

		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<GameComponents.Image>()
				.HasIndex(u => u.Name)
				.IsUnique();
		}
	}
}
