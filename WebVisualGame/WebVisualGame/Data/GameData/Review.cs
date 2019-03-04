using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace WebVisualGame.Data.GameData
{
	public class Review
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
		public int Rating { get; set; }

		[MaxLength(400)]
		[Required]
		public string Comment { get; set; }

		[Required]
		public DateTime Date { get; set; }
	}
}
