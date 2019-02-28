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

		public DbSet<User> Users { get; set; }

		public DbSet<Game> Games { get; set; }

		public DbSet<GameData.SavedGame> SavedGames { get; set; }

		public DbSet<GameData.PointDialog> PointDialogs { get; set; }

		public DbSet<GameData.PointDialogAction> PointDialogActions { get; set; }

		public DbSet<GameData.Transition> Transitions { get; set; }

		public DbSet<GameData.TransitionAction> TransitionActions { get; set; }

		public DbSet<GameData.Condition> Conditions { get; set; }

		public DbSet<TestFile> testFiles { get; set; }
	}
}
