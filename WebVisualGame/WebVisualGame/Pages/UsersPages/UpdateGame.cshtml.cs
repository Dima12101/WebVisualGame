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
		public IFormFile SourceCodeForm { get; set; }

		[BindProperty]
		public string SourceCodeContent { get; set; }

		[BindProperty]
		public string UrlIconContent { get; set; }

		[BindProperty]
		public IFormFile UrlIconForm { get; set; }

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

			ActionForGame = "�������";
			if (Request.Cookies.ContainsKey("GameId"))
			{
				int gameId = Int32.Parse(Request.Cookies["GameId"]);
				Game = db.Games.Find(gameId);
				Title = Game.Title;
				Description = Game.Description;
				UrlIcon = Game.UrlIcon;
				ActionForGame = "��������";
			}
		}

		[HttpPost]
		public async Task<IActionResult> OnPostSubmit()
		{
			UrlIconContent = "HereShouldBeIcon";

			// Perform an initial check to catch FileUpload class
			// attribute violations.

			if (!ModelState.IsValid)
			{
				Response.Cookies.Append("Error", "������ �� �������");
				return RedirectToPage("/UsersPages/UpdateGame");
			}

			SourceCodeContent =
				await FileHelpers.ProcessFormFile(SourceCodeForm, ModelState);

			// Perform a second check to catch ProcessFormFile method
			// violations.
			
			if (!ModelState.IsValid)
			{
				if (ModelState.Keys.Contains(SourceCodeForm.Name))
				{
					string errors = "";
					foreach (var err in ModelState[SourceCodeForm.Name].Errors)
					{
						errors += err.ErrorMessage + "\n";
					}
					Response.Cookies.Append("Error", errors);
				}
				else
				{
					Response.Cookies.Append("Error", "Reading file error!");
				}
				return RedirectToPage("/UsersPages/UpdateGame");
			}

			try
			{
				InputGame();
			}
			catch (Exception e)
			{
				Response.Cookies.Append("Error", e.Message + "\n" +
					((e.InnerException != null) ? e.InnerException.Message : ""));

				return RedirectToPage("/UsersPages/UpdateGame");
			}

			Response.Cookies.Delete("GameId");

			//return RedirectToPage("/UsersPages/UserProfil");
			return RedirectToPage("/Index");

		}

		private void InputGame()
		{
			var gameDbWriter = new GameDbWriter(db);

			if (Request.Cookies.ContainsKey("GameId"))
			{
				int gameId = Int32.Parse(Request.Cookies["GameId"]);
				gameDbWriter.UpdateGame(gameId, Title, SourceCodeContent, Description, UrlIconContent);
			}
			else
			{
				gameDbWriter.SaveNewGame(Title, SourceCodeContent, Description, UrlIconContent,
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
				UrlIcon = Game.PathIcon;