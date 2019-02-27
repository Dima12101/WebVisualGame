using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
    public class CreateGameModel : PageModel
    {
		private readonly Repository db;

		public CreateGameModel(Repository db)
		{
			this.db = db;
		}

		[BindProperty]
		public string Title { get; set; }

		[BindProperty]
		public string Description { get; set; }

		[BindProperty] 
		public IFormFile Icon { get; set; }

		[BindProperty]
		public IFormFile SourceCode { get; set; }


		public void OnGet()
        {
		}

		[HttpPost]
		public IActionResult OnPostCreate()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			int a = 5;
			if (Request.Cookies.ContainsKey("UserId"))
			{
				var UserId = int.Parse(Request.Cookies["UserId"]);
				//var gameDbWriter = new GameDbWriter(db);
				//gameDbWriter.SaveNewGame(Game.Title, Game.UrlIcon, Game.Description, Game.SourceCode, UserId);
			}
			return RedirectToPage("/Index");
		}
	}
}