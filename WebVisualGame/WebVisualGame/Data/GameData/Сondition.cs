using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data.GameData
{
	public class Сondition
	{
		[Key]
		public int Id { get; set; }
		
		[ForeignKey("Transition")]
		[Required]
		public int TransitionId { get; set; }

		[MaxLength(100)]
		[Required]
		public string KeyСondition { get; set; }
	}
}
