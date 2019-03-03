using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;
using WebVisualGame.Utilities;

namespace WebVisualGame.Pages
{
	public class UpdateGmave : PageModel
	{
		[BindProperty]
		public string ActionForGame { get; set; }

		[BindProperty]
		public Game Game { get; set; }

		private readonly Repository db;

		[BindProperty]
		public string Title { get; set; }

		[BindProperty]
		public string Description { get; set; }


		[BindProperty]
		public IFormFile SourceCode { get; set; }

		[BindProperty]
		public string SourceCodeContent { get; set; }

		[BindProperty]
		public string UrlIcon { get; set; }

		[BindProperty]
		public string Error { get; set; }

		public UpdateGmave(Repository db)
		{
			this.db = db;
		}

		public void OnGet()
		{
			SourceCodeContent = Request.Cookies["Content"];
			Error = Request.Cookies["Error"];
			Response.Cookies.Delete("Content");
			Response.Cookies.Delete("Error");

			ActionForGame = "Создать";
			if (Request.Cookies.ContainsKey("GameId"))
			{
				int gameId = Int32.Parse(Request.Cookies["GameId"]);
				Game = db.Games.Find(gameId);
				Title = Game.Title;
				Description = Game.Description;
				UrlIcon = Game.UrlIcon;
				ActionForGame = "Обновить";
			}
		}

		[HttpPost]
		public async Task<IActionResult> OnPostSubmit()
		{
			// Perform an initial check to catch FileUpload class
			// attribute violations.
			if (!ModelState.IsValid)
			{
				Response.Cookies.Append("Error", "Ошибка на клиенте");
				return Page();
			}

			SourceCodeContent =
				await FileHelpers.ProcessFormFile(SourceCode, ModelState);

			// Perform a second check to catch ProcessFormFile method
			// violations.
			if (!ModelState.IsValid)
			{
				Response.Cookies.Append("Error", "Ошибка чтения файла");
				return Page();
			}

			var file = new TestFile();
			file.FileContent = SourceCodeContent;
			InputGame();
			Response.Cookies.Delete("GameId");
			return RedirectToPage("/UsersPages/UserProfil");
		}

		private void InputGame()
		{
			var gameDbWriter = new GameDbWriter(db);

			if (Request.Cookies.ContainsKey("GameId"))
			{
				int gameId = Int32.Parse(Request.Cookies["GameId"]);
				gameDbWriter.UpdateGame(gameId, Title, SourceCodeContent, Description, UrlIcon);
			}
			else
			{
				gameDbWriter.SaveNewGame(Title, SourceCodeContent, Description, UrlIcon,
					Int32.Parse(Request.Cookies["UserId"]));
			}
		}

		public IActionResult OnPostCancel()
		{
			Response.Cookies.Delete("GameId");
			return RedirectToPage("/UsersPages/UserProfil");
		}
	}
}