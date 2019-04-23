using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Data.GameComponents;

namespace WebVisualGame_MVC.Models.PageModels.GameModel
{
	public class MainModel
	{
		public bool IsAuthorize { get; set; }

		public class GameInfo
		{
			public string Title { get; set; }

			public string Description { get; set; }

			public double Rating { get; set; }
		}

		[BindProperty]
		public GameInfo gameInfo { get; set; }

		public class SetReview
		{
			public int Mark { get; set; }

			public string Comment { get; set; }
		}
		public SetReview review { get; set; }

		public class ReviewDisplay
		{
			public string UserName { get; set; }

			public int Mark { get; set; }

			public string Comment { get; set; }

			public DateTime Date { get; set; }
		}
		public List<ReviewDisplay> Reviews { get; set; }

	}
}
