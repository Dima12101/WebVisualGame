using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data
{
	public class Repository : DbContext
	{
		public Repository(DbContextOptions options)
			: base(options)
		{
		}

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	modelBuilder.Entity<Game>()
		//		.HasOne(p => p.User)
		//		.WithMany(t => t.Games)
		//		.OnDelete(DeleteBehavior.Cascade);
		//}

		public DbSet<User> Users { get; set; }

		public DbSet<Game> Games { get; set; }

		public DbSet<GameData.Games> SavedGames { get; set; }

		public DbSet<GameData.PointDialog> PointDialogs { get; set; }

		public DbSet<GameData.PointDialogAction> PointDialogActions { get; set; }

		public DbSet<GameData.Transition> Transitions { get; set; }

		public DbSet<GameData.TransitionAction> TransitionActions { get; set; }

		public DbSet<GameData.Condition> Conditions { get; set; }

		public DbSet<TestFile> testFiles { get; set; }
	}
}
