using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
    public class PlayingModel : PageModel
    {
		[BindProperty]
		public string Login { get; set; }

		[BindProperty]
		public string Password { get; set; }

		private readonly Repository db;

		public PlayingModel(Repository db)
		{
			this.db = db;
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			//db.Users.Add(user);
			//await db.SaveChangesAsync();

			var user = db.Users.FirstOrDefault(i => i.Login == Login && i.Password == Password);

			db.SaveChanges();
			return RedirectToPage("Index");
		}

		public void OnGet()
        {
        }
    }
}