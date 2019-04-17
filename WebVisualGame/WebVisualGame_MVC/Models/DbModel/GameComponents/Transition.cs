using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebVisualGame_MVC.Data.GameComponents
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

		[MaxLength(400)]
		[Required]
		public string Text { get; set; }

		[Required]
		public IList<Condition> Conditions { get; set; }

		[Required]
		public IList<TransitionAction> TransitionActions { get; set; }
	}
}
