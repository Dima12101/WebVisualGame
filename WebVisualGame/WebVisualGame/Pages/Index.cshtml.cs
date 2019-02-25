using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
	public class IndexModel : PageModel
	{
		[BindProperty]
		public string UserName { get; set; }

		private readonly Repository db;

		[BindProperty]
		public IList<Game> games { get; set; }

		public IndexModel(Repository db)
		{
			games = db.Games.ToList();
			isAuthorization = false;
			this.db = db;
		}

		public void OnGet()
		{
			var cond = new Data.GameData.Condition();
				cond.KeyСondition = 5;
				cond.Type = false;

			var trans = new Data.GameData.Transition();
				trans.StartPoint = 19;
				trans.NextPoint = 51;
				trans.GameId = 5;

			trans.Conditions = new List<Data.GameData.Condition>();
			trans.Conditions.Add(cond);
			//db.Conditions.Add(cond);
			db.Transitions.Add(trans);
			db.SaveChanges();
			//var trans3 = db.Transitions.Where(i => i.StartPoint > 3).ToList();
			//var result = (from trans in trans3
			//			  join cond in db.Сonditions on trans.Id equals cond.TransitionId
			//			 select new
			//			 {
			//				 Id = trans.Id,
			//				 NextPoint = trans.NextPoint,
			//				 Type = cond.Type,
			//				 KeyСondition = cond.KeyСondition
			//			 }).ToList();

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

		public IActionResult OnPostStartGame(int gameId)
		{
			return RedirectToPage("/Index");
		}

		public IActionResult OnPostExit()
		{
			Response.Cookies.Delete("Id");
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("Sign");
			isAuthorization = false;
			return RedirectToPage("/Index");
		}
	}
}
