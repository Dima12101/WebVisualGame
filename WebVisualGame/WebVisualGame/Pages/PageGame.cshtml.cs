using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;
using WebVisualGame.Data.GameData;

namespace WebVisualGame.Pages
{
	public class ReviewWithName
	{
		public int UserId { get; set; }

		public string UserName { get; set; }

		public int Mark { get; set; }

		public string Comment { get; set; }

		public DateTime Date { get; set; }
	}

	public class PageGameModel : PageModel
    {
		public Game Game { get; set; }

		public int UserId { get; set; }

		public List<ReviewWithName> Reviews { get; set; }

		[BindProperty]
		public string Comment { get; set; }

		[BindProperty]
		public int Mark { get; set; }

		private readonly Repository db;

		public PageGameModel(Repository db)
		{
			this.db = db;
		}

		public void OnGet()
        {
			int gameId = Int32.Parse(Request.Cookies["GameId"]);
			Game = db.Games.Single(i => i.Id == gameId);
			Reviews = (from review in db.Reviews.Where(i => i.GameId == gameId)
					 join user in db.Users on review.UserId equals user.Id
					 select new
					 {
						 UserId = user.Id,
						 UserName = user.FirstName + " " + user.LastName,
						 Comment = review.Comment,
						 Mark = review.Mark,
						 Date = review.Date
					 }).Select(i => new ReviewWithName
					 {
						UserId = i.UserId,
						UserName = i.UserName,
						Mark = i.Mark,
						Comment = i.Comment,
						Date = i.Date
					 }).ToList();

			if (Request.Cookies.ContainsKey("UserID"))
			{
				UserId = Int32.Parse(Request.Cookies["UserID"]);
				foreach (var review in Reviews)
				{
					if (review.UserId == UserId)
					{
						Comment = review.Comment;
						Mark = review.Mark;
					}
				}
			}
			else
			{
				UserId = 0;
			}
		}

		public IActionResult OnPostSendComment()
		{
			UserId = Int32.Parse(Request.Cookies["UserID"]);
			int gameId = Int32.Parse(Request.Cookies["GameID"]);
			DeleteOldReview(gameId);

			var review = new Review {
				Comment = this.Comment,
				Mark = this.Mark,
				UserId = this.UserId,
				GameId = gameId,
				Date = DateTime.Today
			};
			db.Reviews.Add(review);
			db.SaveChanges();

			CalculateNewRating(gameId);
			db.SaveChanges();
			return RedirectToPage();
		}

		private void DeleteOldReview(int gameId)
		{
			var review = db.Reviews.FirstOrDefault(i => i.GameId == gameId && i.UserId == UserId);
			if (review != null)
			{
				db.Remove(review);
			}
		}

		private void CalculateNewRating(int gameId)
		{
			double newRating = db.Reviews.Where(i => i.GameId == gameId).Average(i => i.Mark);
			Game = db.Games.FirstOrDefault(i => i.Id == gameId);
			Game.Rating = newRating;
			db.Attach(Game).State = EntityState.Modified;
		}

		public IActionResult OnPostStartGame()
		{
			if (Request.Cookies.ContainsKey("UserId"))
			{
				int gameId = Int32.Parse(Request.Cookies["GameID"]);
				UserId = Int32.Parse(Request.Cookies["UserId"]);
				if (db.SavedGames.FirstOrDefault(i => i.GameId == gameId &&
					i.UserId == UserId) == null)
				{
					var savedGame = new SavedGame()
					{
						UserId = UserId,
						State = 0,
						Keys = "",
						GameId = gameId
					};
					db.SavedGames.Add(savedGame);
					db.SaveChanges();
				}
			}
			else
			{
				Response.Cookies.Append("Keys", "");
				Response.Cookies.Append("Point", "0");
			}
			return RedirectToPage("/Playing");
		}
	}
}