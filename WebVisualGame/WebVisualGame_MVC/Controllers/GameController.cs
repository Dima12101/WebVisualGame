using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
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

		private readonly IHostingEnvironment appEnvironment;

		public GameController(DataContext _dataContext, 
			ILogger<HomeController> _logger, 
			IDataProtectionProvider provider,
			IHostingEnvironment _appEnvironment)
		{
			ProtectData.GetInstance().Initialize(provider);
			dataContext = _dataContext;
			appEnvironment = _appEnvironment;
			logger = _logger;
		}

		#region About game
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
		#endregion

		#region Create game
		[Authorize]
		[HttpGet]
		public IActionResult Create()
		{
			logger.LogInformation("Visit /Game/Create page");
			return View(new CreateModel());
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateModel model)
		{
			if (ModelState.IsValid)
			{
				logger.LogInformation($"Began create '{model.Title}'");

				try
				{
					var game = new Game();
					game.UserId = Int32.Parse(HttpContext.User.Identity.Name);
					game.Title = model.Title;
					game.Description = model.Description;
					game.Rating = 0;

					if (model.Icon != null)
					{
						// путь к папке /images/game/
						string pathIcon = "../images/game/" + model.Icon.FileName;
						// сохраняем файл в папку /images/game/ в каталоге wwwroot
						using (var fileStream = new FileStream(appEnvironment.WebRootPath + pathIcon, FileMode.Create))
						{
							await model.Icon.CopyToAsync(fileStream);
						}
						game.PathIcon = pathIcon;
					}
					else
					{
						game.PathIcon = "../images/game/default_icon.ico";
					}

					// путь к папке /files/gameCode/
					string pathCode = "../files/gameCode/" + model.Code.FileName;
					// сохраняем файл в папку /files/gameCode/ в каталоге wwwroot
					using (var fileStream = new FileStream(appEnvironment.WebRootPath + pathCode, FileMode.Create))
					{
						await model.Code.CopyToAsync(fileStream);
					}
					string code = "";
					// считываем переданный файл в string
					using (var reader = new StreamReader(model.Code.OpenReadStream(), detectEncodingFromByteOrderMarks: true))
					{
						code = await reader.ReadToEndAsync();
					}
					// парсинг кода и сохраанения в БД
					var gameDbWriter = new GameDbWriter(dataContext);
					gameDbWriter.SaveGameComponents(code, game);
					game.PathCode = pathCode;

					dataContext.Games.Add(game);
					dataContext.SaveChanges();
					return Redirect("~/Home/Index");
				}
				catch (Exception ex)
				{
					logger.LogError(ex.Message);
					throw ex;
				}
			}
			return View(model);
		}
		#endregion

		#region Redaction game
		[HttpGet]
		public IActionResult Redaction(string gameIdEncode)
		{
			logger.LogInformation("Visit /Game/Redaction page");

			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			logger.LogInformation("GameId: " + gameIdDecoded);
			Response.Cookies.Append("GameId", gameIdDecoded);

			return View();
		}
		#endregion

		[HttpPost]
		public IActionResult Delete(string gameIdEncode)
		{
			logger.LogInformation("Visit /Game/Delete action");

			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			var gameDbWriter = new GameDbWriter(dataContext);
			gameDbWriter.DeleteGame(gameId);

			return Redirect("~/Account/Profile");
		}
	}
}