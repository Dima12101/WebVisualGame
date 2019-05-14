using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebVisualGame_MVC.Data
{
	public class Game
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("User")]
		public int? UserId { get; set; }
		public User User { get; set; }

		[MaxLength(100)]
		[Required]
		public string Title { get; set; }

		[MaxLength(300)]
		public string Description { get; set; }

		[MaxLength(100)]
		[Required]
		public string PathIcon { get; set; }

		[Required]
		public double Rating { get; set; }

		[MaxLength(1000)]
		[Required]
		public string PathCode { get; set; }

		[Required]
		public DateTime Date { get; set; }

		//ДЛя процесса игры

		public IList<GameComponents.PointDialog> PointDialogues { get; set; }

		public IList<GameComponents.Transition> Transitions { get; set; }


		public IList<GameComponents.SavedGame> SavedGames { get; set; }

		public IList<GameComponents.Review> Reviews { get; set; }
	}
}
