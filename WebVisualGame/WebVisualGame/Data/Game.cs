using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data
{
	public class Game
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("User")]
		[Required]
		public int UserId { get; set; }

		[MaxLength(100)]
		[Required]
		public string Title { get; set; }

		[MaxLength(300)]
		public string Description { get; set; }

		[Url]
		[MaxLength(100)]
		[Required]
		public string UrlIcon { get; set; }

		[Required]
		public double Rating { get; set; }

		[MaxLength(1000)]
		[Required]
		public string SourceCode { get; set; }

		[Required]
		public DbSet<GameData.PointDialogue> pointDialogues { get; set; }

		[Required]
		public DbSet<GameData.Transition> transitions { get; set; }
	}
}
