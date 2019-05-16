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
		public class GameInfo
		{
			public int Id { get; set; }

			public string Title { get; set; }

			public string Description { get; set; }

			public double Rating { get; set; }

			public DateTime Data { get; set; }
		}
		public GameInfo Game { get; set; }

		public class SetReview
		{
			public int Mark { get; set; }

			public string Comment { get; set; }
		}
		public SetReview Review { get; set; }

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
