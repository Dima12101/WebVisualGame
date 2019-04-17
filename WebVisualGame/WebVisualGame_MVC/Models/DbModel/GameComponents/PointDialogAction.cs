using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebVisualGame_MVC.Data.GameComponents
{
	public class PointDialogAction
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("PointDialog")]
		[Required]
		public int PointDialogId { get; set; }

		[Required]
		public bool Type { get; set; }

		[Required]
		public int KeyAction { get; set; }
	}
}
