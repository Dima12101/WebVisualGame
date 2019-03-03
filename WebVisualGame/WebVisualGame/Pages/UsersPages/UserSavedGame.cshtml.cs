using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;

namespace WebVisualGame.Pages.UsersPages
{
    public class UserSavedGameModel : PageModel
    {
		[BindProperty]
		public IList<Game> games { get; set; }

		private readonly Repository db;

		public UserSavedGameModel(Repository db)
		{
			this.db = db;
		}

		public void OnGet()
        {
			int userId = Int32.Parse(Request.Cookies["UserId"]);


			games = db.Games.Where(i => i.UserId == userId).ToList();
		}


		public IActionResult OnPostDeleteGame(int gameId)
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			db.SavedGames.Remove(db.SavedGames.FirstOrDefault(i => i.GameId == gameId &&
				i.UserId == userId));

			db.SaveChanges();
			return RedirectToPage();
		}

		public IActionResult OnPostStartGame(int gameId)
		{
			Response.Cookies.Append("GameId", gameId.ToString());
			return RedirectToPage("/Playing");
		}

	}
}