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
		public User user { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			if (Request.Cookies.ContainsKey("UserId"))
			{
				var UserId = int.Parse(Request.Cookies["UserId"]);
				user = await db.Users.FindAsync(UserId);
			}
			return Page();
		}

		[BindProperty]
		public List<string> ServerErrors { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (db.Users.FirstOrDefault(i => (i.Login == user.Login) && (i.Id != user.Id)) != null)
				ServerErrors.Add("Пользователь с таким логином уже существует");
			if (db.Users.FirstOrDefault(i => (i.Email == user.Email) && (i.Id != user.Id)) != null)
				ServerErrors.Add("Пользователь с таким адресом эл.почты уже существует");

			if (ServerErrors.Count != 0)
			{
				return Page();
			}
			else
			{
				db.Attach(user).State = EntityState.Modified;

				Response.Cookies.Delete("Login");
				Response.Cookies.Append("Login", user.Login);

				Response.Cookies.Delete("Sign");
				Response.Cookies.Append("Sign", SignGenerator.GetSign(user.Login + "bytepp"));

				try
				{
					await db.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					throw new Exception($"User {user.Id} not found!");
				}
				return RedirectToPage("/UsersPages/UserProfil");
			}
		}
	}
}