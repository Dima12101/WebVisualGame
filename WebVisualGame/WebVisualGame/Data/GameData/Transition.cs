using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data.GameData
{
	public class Transition
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Game")]
		[Required]
		public int GameId { get; set; }

		[Required]
		public int StartPoint { get; set; }

		[Required]
		public int NextPoint { get; set; }

		[Required]
		public int NumberInList { get; set; }

		[Required]
		public DbSet<GameData.Сondition> сonditions { get; set; }
	}
}
