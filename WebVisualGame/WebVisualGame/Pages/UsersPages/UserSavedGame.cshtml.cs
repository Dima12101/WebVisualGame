using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;
using WebVisualGame.Data.GameData;

namespace WebVisualGame.Pages.UsersPages
{
    public class UserSavedGameModel : PageModel
    {
		[BindProperty]
		public List<Game> games { get; set; }

		private readonly Repository db;

		public UserSavedGameModel(Repository db)
		{
			this.db = db;
		}

		public void OnGet()
        {
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			games = (from save in db.SavedGames.Where(i => i.UserId == userId)
						 join game in db.Games on save.GameId equals game.Id
						 select new
						 {
							 Id = game.Id,
							 Title = game.Title,
							 Description = game.Description,
							 Rating = game.Rating,
							 UrlIcon = game.UrlIcon
						 }).Select(i => new Game
						 {
							 Id = i.Id,
							 Description = i.Description,
							 Title = i.Title,
							 Rating = i.Rating,
							 UrlIcon = i.UrlIcon
						 }).ToList();
		}


		public IActionResult OnPostDeleteGame(int gameId)
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			var save = db.SavedGames.FirstOrDefault(i => i.GameId == gameId && i.UserId == userId);
			db.SavedGames.Remove(save);

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
							 UrlIcon = game.PathIcon
							 PathIcon = i.UrlIcon