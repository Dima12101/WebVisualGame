using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;
using WebVisualGame.Data.GameData;
using WebVisualGame.Utilities;

namespace WebVisualGame.Pages.UsersPages
{
    public class UserProfilModel : PageModel
    {
		[BindProperty]
		public int AccessLevel { get; set; }

		[BindProperty]
		public string UserName { get; set; }

		[BindProperty]
		public bool InputField { get; set; }

		public User user;

		[BindProperty]
		public IList<Game> games { get; set; }

		[BindProperty]
		public Game _Game { get; set; }

		private readonly Repository db;

		public UserProfilModel(Repository db)
		{
			this.db = db;
		}

		public void OnGet()
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			user = db.Users.FirstOrDefault(i => i.Id == userId);
			UserName = user.FirstName + " " + user.LastName;

			games = db.Games.Where(i => i.UserId == user.Id).ToList();
		}

		public IActionResult OnPostDeleteGame(string gameId)
		{
			var gameIdDecoded = ProtectData.GetInstance().DecodeToInt(gameId);
			var gameDbWriter = new GameDbWriter(db);
			gameDbWriter.DeleteGame(gameIdDecoded);
			return RedirectToPage();
		}

		public IActionResult OnPostUpdateGame(string gameId)
		{
			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameId);
			Response.Cookies.Append("GameId", gameIdDecoded);
			return RedirectToPage("/UsersPages/Updategame");
		}

		public IActionResult OnPostNewGame()
		{
			Response.Cookies.Delete("GameId");
			return RedirectToPage("/UsersPages/Updategame");
		}

		public IActionResult OnPostStartGame(string gameId)
		{
			var gameIdDecoded = ProtectData.GetInstance().DecodeToInt(gameId);
			Response.Cookies.Append("GameId", gameIdDecoded.ToString());

			int userId = Int32.Parse(Request.Cookies["UserId"]);
			if (db.SavedGames.FirstOrDefault(i => i.GameId == gameIdDecoded &&
				i.UserId == userId) == null)
			{
				var savedGame = new SavedGame()
				{
					UserId = userId,
					State = 0,
					Keys = "",
					GameId = gameIdDecoded
				};
				db.SavedGames.Add(savedGame);
				db.SaveChanges();
			}
			return RedirectToPage("/Playing");
		}

		public IActionResult OnPostExit()
		{
			Response.Cookies.Delete("UserId");
			Response.Cookies.Delete("GameId");
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("Sign");
			return RedirectToPage("/Index");
		}

		public IActionResult OnPostRedaction()
		{
			return RedirectToPage("/UsersPages/UserRedactionProfil");
		}

		public IActionResult OnPostContinueGame()
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			if (db.SavedGames.FirstOrDefault(i => i.UserId == userId) == null)
			{
				return Page();
			}
			return RedirectToPage("/UsersPages/UserSavedGame");
		}
	}
}