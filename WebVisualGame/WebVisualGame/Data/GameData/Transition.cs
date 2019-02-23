using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data.GameData
{
	public class Transition
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int StartPoint { get; set; }

		[Required]
		public int NextPoint { get; set; }

		[Required]
		public int NumberInList { get; set; }
	}
}
