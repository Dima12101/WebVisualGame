using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebVisualGame.Pages
{
	public class ListUsersModel : PageModel
	{
		private readonly Repository _db;

		public ListUsersModel(Repository db)
		{
			_db = db;
		}

		[BindProperty]
		public int AdminId { get; set; }

		public IList<User> Users { get; private set; }

		public async Task OnGetAsync()
		{
			if (Request.Cookies.ContainsKey("Id"))
			{
				AdminId = System.Int32.Parse(Request.Cookies["Id"]);
			}
			Users = await _db.Users.AsNoTracking().ToListAsync();
		}

		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var user = await _db.Users.FindAsync(id);

			if (user != null)
			{
				_db.Users.Remove(user);
				await _db.SaveChangesAsync();
			}
			return RedirectToPage();
		}
	}
}