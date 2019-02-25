using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data.GameData
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
