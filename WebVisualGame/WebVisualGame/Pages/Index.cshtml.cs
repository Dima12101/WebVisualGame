using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;

namespace WebVisualGame.Pages
{
	public class IndexModel : PageModel
	{
		private readonly Repository db;

		public IndexModel(Repository db)
		{
			isAuthorization = false;
			this.db = db;
		}

		public void OnGet()
		{
			if (Request.Cookies.ContainsKey("Login") && Request.Cookies.ContainsKey("Sign"))
			{
				var login = Request.Cookies["Login"];
				var sign = Request.Cookies["Sign"];
				if (sign == SignGenerator.GetSign(login + "bytepp"))
				{
					isAuthorization = true;
				}
			}
		}

		[BindProperty]
		public bool isAuthorization { get; set; }

		public IActionResult OnPostExit()
		{
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("Sign");
			isAuthorization = false;
			return Page();
		}
	}
}
