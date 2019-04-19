using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Data.GameComponents;
using WebVisualGame_MVC.Models.PageModels.GameModel;
using WebVisualGame_MVC.Utilities;

namespace WebVisualGame_MVC.Controllers
{
    public class GameController : Controller
    {
		private readonly ILogger logger;

		private readonly DataContext dataContext;

		public GameController(DataContext _dataContext, ILogger<HomeController> _logger, IDataProtectionProvider provider)
		{
			ProtectData.GetInstance().Initialize(provider);
			dataContext = _dataContext;
			logger = _logger;
		}

		[HttpGet]
		public IActionResult Main(string gameIdEncode)
        {
			logger.LogInformation("Visit /Game/Main page");

			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);

			logger.LogInformation("GameId: " + gameIdDecoded.ToString());

			var mainModel = new MainModel();
			try
			{
				var gameId = Int32.Parse(gameIdDecoded);
				mainModel.game = dataContext.Games.Single(i => i.Id == gameId);
				
				if (HttpContext.User.Identity.IsAuthenticated)
				{
					logger.LogInformation($"Trying get user by id");
					var userId = Int32.Parse(HttpContext.User.Identity.Name);
					logger.LogInformation("UserId: " + userId.ToString());

					mainModel.user = dataContext.Users.Single(i => i.Id == userId);
					mainModel.review = new MainModel.SetReview();
				}

				mainModel.Reviews = (from review in dataContext.Reviews.Where(i => i.GameId == gameId)
						   join user in dataContext.Users on review.UserId equals user.Id
						   select new
						   {
							   UserName = user.FirstName + " " + user.LastName,
							   Comment = review.Comment,
							   Mark = review.Mark,
							   Date = review.Date
						   }).Select(i => new MainModel.ReviewDisplay
						   {
							   UserName = i.UserName,
							   Mark = i.Mark,
							   Comment = i.Comment,
							   Date = i.Date
						   }).ToList();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw ex;
			}
			return View(mainModel);
        }

		[Authorize]
		[HttpPost]
		public IActionResult SetReview(MainModel.SetReview review)
		{
			logger.LogInformation("Set review. ");

			var oldReview = dataContext.Reviews.FirstOrDefault(i => i.GameId == review.GameId && i.UserId == review.UserId);
			if (oldReview != null)
			{
				oldReview.Comment = review.Comment;
				oldReview.Mark = review.Mark;
				oldReview.Date = DateTime.Today;
				dataContext.Attach(oldReview).State = EntityState.Modified;
			}
			else
			{
				var newReview = new Review
				{
					Comment = review.Comment,
					Mark = review.Mark,
					UserId = review.UserId,
					GameId = review.GameId,
					Date = DateTime.Today
				};
				dataContext.Reviews.Add(newReview);
			}	
			dataContext.SaveChanges();

			double newRating = dataContext.Reviews.Where(i => i.GameId == review.GameId).Average(i => i.Mark);
			var game = dataContext.Games.FirstOrDefault(i => i.Id == review.GameId);
			game.Rating = newRating;
			dataContext.Attach(game).State = EntityState.Modified;
			dataContext.SaveChanges();

			return RedirectToAction("Main", new { gameIdEncode = ProtectData.GetInstance().Encode(review.GameId) });
		}

	}
}