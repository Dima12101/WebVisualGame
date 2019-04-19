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
		public User user { get; set; }

		public Game game { get; set; }

		public class SetReview
		{
			public int UserId { get; set; }

			public int GameId { get; set; }

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
