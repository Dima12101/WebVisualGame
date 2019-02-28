using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebVisualGame.Data;
using WebVisualGame.Utilities;

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

		//[BindProperty] 
		//public IFormFile Icon { get; set; }
		
		[BindProperty]
		public IFormFile SourceCode { get; set; }

		[BindProperty]
		public string SourceCodeContent { get; set; }
		
		public void OnGet()
        {
			SourceCodeContent = Request.Cookies["Content"];
			Error = Request.Cookies["Error"];
			Response.Cookies.Delete("Content");
			Response.Cookies.Delete("Error");
		}

		[BindProperty]
		public string Error { get; set; }

		[HttpPost]
		public async Task<IActionResult> OnPostAsync()
		{
			// Perform an initial check to catch FileUpload class
            // attribute violations.
            if (!ModelState.IsValid)
            {
				Response.Cookies.Append("Error", "Ошибка на клиенте");
				return Page();
            }

            SourceCodeContent = 
                await FileHelpers.ProcessFormFile(SourceCode, ModelState);

			Response.Cookies.Append("Content", SourceCodeContent);

			// Perform a second check to catch ProcessFormFile method
			// violations.
			if (!ModelState.IsValid)
            {
				Response.Cookies.Append("Error", "Ошибка чтения файла");
				return Page();
            }

			if (Request.Cookies.ContainsKey("UserId"))
			{
				var UserId = int.Parse(Request.Cookies["UserId"]);
				//var gameDbWriter = new GameDbWriter(db);
				//gameDbWriter.SaveNewGame(Title, "", Description, SourceCodeContent, UserId);
			}

			var file = new TestFile();
			file.FileContent = SourceCodeContent;
			db.testFiles.Add(file);
			await db.SaveChangesAsync();

			return RedirectToPage();
		}
	}
}