using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebVisualGame_MVC.Models.DbModel.GameComponents
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
		public int Mark { get; set; }

		[MaxLength(400)]
		public string Comment { get; set; }

		[Required]
		public DateTime Date { get; set; }
	}
}
