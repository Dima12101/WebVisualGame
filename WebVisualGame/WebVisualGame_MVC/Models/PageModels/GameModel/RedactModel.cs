using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame_MVC.Models.PageModels.GameModel
{
	public class RedactModel
	{
		public int GameId { get; set; }

		public string PathIcon { get; set; }

		public class BaseInfo
		{
			public IFormFile Icon { get; set; }

			public string Title { get; set; }

			public string Description { get; set; }
		}
		public BaseInfo baseInfo { get; set; }

		public class CodeInfo
		{
			public List<string> Messages { get; set; }

			public string CodeText { get; set; }

			public IFormFile CodeFile { get; set; }
		}
		public CodeInfo codeInfo { get; set; }

		public class BindImage
		{
			public string Name { get; set; }

			public IFormFile File { get; set; }

			public string Path { get; set; }
		}
		public List<BindImage> bindImages { get; set; }

	}
}
