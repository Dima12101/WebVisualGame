using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebVisualGame_MVC.Data.GameComponents
{
	public class PointDialog
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Game")]
		[Required]
		public int GameId { get; set; }

		[Required]
		public int StateNumber { get; set; }

		[MaxLength(300)]
		[Required]
		public string Text { get; set; }

		[ForeignKey("Image")]
		public int? ImageId { get; set; }
		public Image Image { get; set; }
	}
}
