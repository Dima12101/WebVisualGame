using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data.GameData
{
	public class PointDialogue
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int StateNumber { get; set; }

		[MaxLength(300)]
		[Required]
		public string Text { get; set; }

		[MaxLength(100)]
		[Required]
		public string Background_imageURL { get; set; }


	}
}
