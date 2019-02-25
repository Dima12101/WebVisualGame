using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;

namespace WebVisualGame.Pages.AdminPages
{
	public class EditUserModel : PageModel
	{
		private readonly Repository _db;

		public EditUserModel(Repository db)
		{
			ServerErrors = new List<string>();
			_db = db;
		}

		[BindProperty]
		public List<string> ServerErrors { get; set; }

		[BindProperty]
		public User user { get; set; }

		public async Task<IActionResult> OnGetAsync(int id)
		{
			user = await _db.Users.FindAsync(id);

			if (user == null)
			{
				return RedirectToPage("/AdminPages/ListUsers");
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (_db.Users.FirstOrDefault(i => (i.Login == user.Login) && (i.Id != user.Id)) != null)
				ServerErrors.Add("Пользователь с таким логином уже существует");
			if (_db.Users.FirstOrDefault(i => (i.Email == user.Email) && (i.Id != user.Id)) != null)
				ServerErrors.Add("Пользователь с таким адресом эл.почты уже существует");

			if (ServerErrors.Count != 0)
			{
				return Page();
			}
			else
			{
				_db.Attach(user).State = EntityState.Modified;

				try
				{
					await _db.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					throw new Exception($"User {user.Id} not found!");
				}
				return RedirectToPage("/AdminPages/ListUsers");
			}
		}
	}
}