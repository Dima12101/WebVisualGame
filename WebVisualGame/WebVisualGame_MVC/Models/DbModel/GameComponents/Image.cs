using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame_MVC.Data.GameComponents
{
    public class Image
    {
		[Key]
		public int Id { get; set; }

		[ForeignKey("Game")]
		public int? GameId { get; set; }
		public Game Game { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Path { get; set; }
	}
}
