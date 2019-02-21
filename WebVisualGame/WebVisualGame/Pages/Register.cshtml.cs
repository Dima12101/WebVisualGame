using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
    public class RegisterModel : PageModel
    {
		private readonly Repository db;

		public RegisterModel(Repository db)
		{
			this.db = db;
		}

		[BindProperty]
		public User user { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			db.Users.Add(user);
			await db.SaveChangesAsync();

			return RedirectToPage("Index");
		}
	}
}