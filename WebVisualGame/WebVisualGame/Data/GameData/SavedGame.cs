using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data.GameData
{
	public class SavedGame
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("User")]
		[Required]
		public int UserId { get; set; }

		[ForeignKey("Game")]
		[Required]
		public int GameId { get; set; }

		[Required]
		public int State { get; set; }

		[MaxLength(200)]
		[Required]
		public string Keys { get; set; }
	}
}
