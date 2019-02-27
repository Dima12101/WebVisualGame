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

		[BindProperty]
		public string ActionForGame { get; set; }

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

			ActionForGame = "Создать";
			if (Request.Cookies.ContainsKey("GameId"))
			{
				var gameId = int.Parse(Request.Cookies["GameId"]);
				_Game = db.Games.Find(gameId);
				ActionForGame = "Обновить";
			}

			InputField = Request.Cookies.ContainsKey("InputField");
			if (!InputField)
			{
				games = db.Games.Where(i => i.UserId == user.Id).ToList();
			}
		}

		public IActionResult OnPostDeleteGame(int gameId)
		{
			var gameDbWriter = new GameDbWriter(db);
			gameDbWriter.DeleteGame(gameId);
			return RedirectToPage();
		}

		public IActionResult OnPostUpdateGame(int gameId)
		{
			Response.Cookies.Append("GameId", gameId.ToString());
			Response.Cookies.Append("InputField", "");

			return RedirectToPage();
		}

		public IActionResult OnPostNewGame()
		{
			Response.Cookies.Delete("GameId");
			Response.Cookies.Append("InputField", "");
			return RedirectToPage();
		}

		public IActionResult OnPostInputGame()
		{
			var gameDbWriter = new GameDbWriter(db);
			
			if (Request.Cookies.ContainsKey("GameId"))
			{
				Response.Cookies.Delete("InputField");
				int gameId = Int32.Parse(Request.Cookies["GameId"]);
				gameDbWriter.UpdateGame(gameId, _Game.Title, _Game.SourceCode, _Game.Description, _Game.UrlIcon);
			}
			else
			{
				Response.Cookies.Delete("InputField");
				gameDbWriter.SaveNewGame(_Game.Title, _Game.SourceCode, _Game.Description, _Game.UrlIcon,
					Int32.Parse(Request.Cookies["UserId"]));
			}
			return RedirectToPage();
		}

		public IActionResult OnPostCancel()
		{
			Response.Cookies.Delete("GameId");
			Response.Cookies.Delete("InputField");
			return RedirectToPage();
		}

		public IActionResult OnPostStartGame(int gameId)
		{
			Response.Cookies.Append("GameId", gameId.ToString());
			Response.Cookies.Append("SetKeys", "");
			Response.Cookies.Append("StartPoint", "0");
			return RedirectToPage("/Playing");
		}

		public IActionResult OnPostExit()
		{
			Response.Cookies.Delete("UserId");
			Response.Cookies.Delete("InputField");
			Response.Cookies.Delete("GameId");
			Response.Cookies.Delete("Id");
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("Sign");
			return RedirectToPage("/Index");
		}

		public IActionResult OnPostRedaction()
		{
			return RedirectToPage("/UsersPages/UserRedactionProfil");
		}
	}
}