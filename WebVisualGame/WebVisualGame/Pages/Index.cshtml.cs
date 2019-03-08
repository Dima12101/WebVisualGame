using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;
using WebVisualGame.Utilities;

namespace WebVisualGame.Pages
{
	public class IndexModel : PageModel
	{
		[BindProperty]
		public string UserName { get; set; }

		private readonly Repository db;

		[BindProperty]
		public IList<Game> games { get; set; }

		public IndexModel(Repository db, IDataProtectionProvider provider)
		{
			ProtectData.GetInstance().Initialize(provider);

			var gameDbWriter = new GameDbWriter(db);
			games = db.Games.ToList();
			isAuthorization = false;
			this.db = db;
		}

		public void OnGet()
		{
			if (Request.Cookies.ContainsKey("Login") && Request.Cookies.ContainsKey("Sign"))
			{
				var login = Request.Cookies["Login"];
				var sign = Request.Cookies["Sign"];
				if (sign == SignGenerator.GetSign(login + "bytepp"))
				{
					isAuthorization = true;
					var user = db.Users.FirstOrDefault(i => i.Login == login);
					UserName = user.FirstName + " " + user.LastName;
					if (user.AccessLevel == 1)
					{
						isAdmin = true;
					}
					else
					{
						isAdmin = false;
					}
				}
			}
		}

		[BindProperty]
		public bool isAuthorization { get; set; }

		[BindProperty]
		public bool isAdmin { get; set; }

		public IActionResult OnPostAboutGame(string gameId)
		{
			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameId);
			Response.Cookies.Append("GameId", gameIdDecoded);
			return RedirectToPage("/PageGame");
		}

		public IActionResult OnPostExit()
		{
			Response.Cookies.Delete("UserId");
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("Sign");
			isAuthorization = false;
			return RedirectToPage("/Index");
		}
	}
}
