using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebVisualGame_MVC.Data;

namespace WebVisualGame_MVC.Models.PageModels
{
	public class IndexModel
	{
		private readonly DataContext dataContext;

		[BindProperty]
		public User User { get; private set; }

		public List<Game> Games { get; private set; }

		public IndexModel(DataContext _dataContext)
		{
			dataContext = _dataContext;

			Games = dataContext.Games.ToList();
		}

		public void SetUser(int userId)
		{
			User = dataContext.Users.FirstOrDefault(i => i.Id == userId);
		}
	}
}
