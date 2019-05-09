using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebVisualGame_MVC.Data;

namespace WebVisualGame_MVC.Models.PageModels
{
	public class IndexModel
	{
		public class GameInfo
		{
			public int Id { get; set; }

			public string Title { get; set; }

			public double Rating { get; set; }

			public string PathIcon { get; set; }
		}
		public enum TypeSelect { Last = 0, Best = 1 }

		public TypeSelect CurrentTypeSelect { get; set; }
		public List<GameInfo> Games { get; set; }
	}
}
