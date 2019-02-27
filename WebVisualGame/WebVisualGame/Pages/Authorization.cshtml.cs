using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Web;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
    public class AuthorizationModel : PageModel
    {
		[BindProperty]
		public string Login { get; set; }

		[BindProperty]
		public string Password { get; set; }

		private readonly Repository db;

		public AuthorizationModel(Repository db)
		{
			this.db = db;
		}

		[BindProperty]
		public string Error { get; set; }

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = db.Users.FirstOrDefault(i => i.Login == Login && i.Password == Password);
			if (user == null)
			{
				Error = "Такой пользователь не существует!";
				return Page();
			}
			else
			{
				Response.Cookies.Append("Id", user.Id.ToString());
				Response.Cookies.Append("Login", Login);
				Response.Cookies.Append("Sign", SignGenerator.GetSign(Login + "bytepp"));
				db.SaveChanges();
				return RedirectToPage("UserProfil");
			}
		}
	}
}