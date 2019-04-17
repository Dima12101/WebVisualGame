using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebVisualGame_MVC.Data.GameComponents
{
	public class TransitionAction
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Transition")]
		[Required]
		public int TransitionId { get; set; }

		[Required]
		public bool Type { get; set; }

		[Required]
		public int KeyAction { get; set; }
	}
}
