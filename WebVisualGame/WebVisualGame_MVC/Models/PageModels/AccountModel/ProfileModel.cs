using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame_MVC.Models.PageModels.AccountModel
{
	public class ProfileModel
	{
		public string UserName { get; set; }

		public string Email { get; set; }

		public string PathUserAvatar { get; set; }

		public DateTime Data { get; set; }

		public int AccessLevel { get; set; }

		public class UserGame
		{
			public int Id { get; set; }

			public string Title { get; set; }

			public double Rating { get; set; }

			public string PathIcon { get; set; }
		}
		public List<UserGame> UserGames { get; set; }
	}
}
