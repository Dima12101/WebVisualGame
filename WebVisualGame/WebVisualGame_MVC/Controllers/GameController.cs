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

		private MainModel Get_mainModel(int gameId)
		{
			var mainModel = new MainModel();
			try
			{
				var game = dataContext.Games.Single(i => i.Id == gameId);

				mainModel.gameInfo = new MainModel.GameInfo
				{
					Title = game.Title,
					Description = game.Description,
					Rating = game.Rating
				};


				mainModel.IsAuthorize = HttpContext.User.Identity.IsAuthenticated;
				mainModel.review = new MainModel.SetReview();

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
			return mainModel;
		}

		[HttpGet]
		public IActionResult Main(string gameIdEncode)
        {
			logger.LogInformation("Visit /Game/Main page");

			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			logger.LogInformation("GameId: " + gameIdDecoded);
			Response.Cookies.Append("GameId", gameIdDecoded);

			return View(Get_mainModel(gameId));
        }

		[Authorize]
		[HttpPost]
		public IActionResult SetReview(MainModel model)
		{
			logger.LogInformation("Set review. ");
			try
			{
				var userId = Int32.Parse(HttpContext.User.Identity.Name);
				var gameId = Int32.Parse(Request.Cookies["GameId"]);

				//Проверка на наличие комментария
				var oldReview = dataContext.Reviews.FirstOrDefault(i => i.GameId == gameId && i.UserId == userId);
				if (oldReview != null)
				{
					//Изменяем имеющийся
					oldReview.Comment = model.review.Comment;
					oldReview.Mark = model.review.Mark;
					oldReview.Date = DateTime.Today;
					dataContext.Attach(oldReview).State = EntityState.Modified;
				}
				else
				{
					//Добавляем новый
					var newReview = new Review
					{
						Comment = model.review.Comment,
						Mark = model.review.Mark,
						UserId = userId,
						GameId = gameId,
						Date = DateTime.Today
					};
					dataContext.Reviews.Add(newReview);
				}
				dataContext.SaveChanges();

				//Новый рейтинг
				double newRating = dataContext.Reviews.Where(i => i.GameId == gameId).Average(i => i.Mark);
				var game = dataContext.Games.FirstOrDefault(i => i.Id == gameId);
				game.Rating = newRating;
				dataContext.Attach(game).State = EntityState.Modified;
				dataContext.SaveChanges();
				//---

				return View("Main", Get_mainModel(gameId));
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw ex;
			}
		}

	}
}