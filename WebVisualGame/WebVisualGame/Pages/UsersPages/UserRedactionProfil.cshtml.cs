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
    public class UserRedactionProfilModel : PageModel
    {
		private readonly Repository db;

		public UserRedactionProfilModel(Repository db)
		{
			ServerErrors = new List<string>();
			this.db = db;
		}

		[BindProperty]
		public User User { get; set; }

		[BindProperty]
		public List<string> ServerErrors { get; set; }

		public IActionResult OnPost()
		{

			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (db.Users.FirstOrDefault(i => i.Login == User.Login) != null)
				ServerErrors.Add("Пользователь с таким логином уже существует");
			if (db.Users.FirstOrDefault(i => i.Email == User.Email) != null)
				ServerErrors.Add("Пользователь с таким адресом эл.почты уже существует");

			if (ServerErrors.Count != 0)
			{
				return Page();
			}
			else
			{
				User.Id = Int32.Parse(Request.Cookies["Id"]);
				Response.Cookies.Append("Login", User.Login);
				Response.Cookies.Append("Sign", SignGenerator.GetSign(User.Login + "bytepp"));
				db.Users.UpdateRange(User);
				db.SaveChangesAsync();
				return RedirectToPage("UserProfil");
			}
		}
	}
}