using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
    public class RegistrationModel : PageModel
    {
		private readonly Repository db;

		public RegistrationModel(Repository db)
		{
			ServerErrors = new List<string>();
			this.db = db;
		}

		[BindProperty]
		public User user { get; set; }

		[BindProperty]
		public List<string> ServerErrors { get; set; }

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (db.Users.FirstOrDefault(i => i.Login == user.Login) != null)
				ServerErrors.Add("Пользователь с таким логином уже существует");
			if (db.Users.FirstOrDefault(i => i.Email == user.Email) != null)
				ServerErrors.Add("Пользователь с таким адресом эл.почты уже существует");

			if (ServerErrors.Count != 0)
			{
				return Page();
			}
			else
			{
				db.Users.Add(user);
				db.SaveChangesAsync();
				return RedirectToPage("Index");
			}
		}
	}
}