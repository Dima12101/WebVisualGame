using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data
{
	public class TestFile
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string FileContent { get; set; }
	}
}
