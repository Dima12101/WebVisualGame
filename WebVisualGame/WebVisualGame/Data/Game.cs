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
		public int? UserId { get; set; }
		public User User { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Введите название игры")]
		public string Title { get; set; }

		[MaxLength(300)]
		public string Description { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Прикрепите иконку игры")]
		public string PathIcon { get; set; }

		[Required]
		public double Rating { get; set; }

		[MaxLength(1000)]
		[Required(ErrorMessage = "Прикрепите код игры")]
		public string PathCode { get; set; }

		public IList<GameData.PointDialog> PointDialogues { get; set; }

		public IList<GameData.Transition> Transitions { get; set; }
		
		public IList<GameData.SavedGame> SavedGames { get; set; }

		public IList<GameData.Review> Reviews { get; set; }
	}
}
