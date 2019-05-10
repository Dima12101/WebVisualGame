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

		private PlayModel model;

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

				mainModel.Game = new MainModel.GameInfo
				{
					Id = gameId,
					Title = game.Title,
					Description = game.Description,
					Rating = game.Rating
				};


				mainModel.Review = new MainModel.SetReview();

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

			return View(Get_mainModel(gameId));
        }

		[Authorize]
		[HttpPost]
		public IActionResult SetReview(MainModel.SetReview model, string gameIdEncode)
		{
			logger.LogInformation("Set review. ");
			try
			{
				var userId = Int32.Parse(HttpContext.User.Identity.Name);

				var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
				var gameId = Int32.Parse(gameIdDecoded);

				//Проверка на наличие комментария
				var oldReview = dataContext.Reviews.FirstOrDefault(i => i.GameId == gameId && i.UserId == userId);
				if (oldReview != null)
				{
					//Изменяем имеющийся
					oldReview.Comment = model.Comment;
					oldReview.Mark = model.Mark;
					oldReview.Date = DateTime.Now;
					dataContext.Attach(oldReview).State = EntityState.Modified;
				}
				else
				{
					//Добавляем новый
					var newReview = new Review
					{
						Comment = model.Comment,
						Mark = model.Mark,
						UserId = userId,
						GameId = gameId,
						Date = DateTime.Now
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

						string pathIcon = "/images/game/" + model.Icon.FileName.Split("\\").Last();
						// сохраняем файл в папку /images/game/ в каталоге wwwroot
						using (var fileStream = new FileStream(appEnvironment.WebRootPath + pathIcon, FileMode.Create))
						{
							await model.Icon.CopyToAsync(fileStream);
						}
						game.PathIcon = ".." + pathIcon;
					}
					else
					{
						game.PathIcon = "../images/game/default_icon.ico";
					}

					// путь к папке /files/gameCode/
					string pathCode = "/files/gameCode/" + model.Code.FileName.Split("\\").Last();
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

					// парсинг кода и сохранения в БД
					var gameComponentsWorker = new GameComponentsWorker(dataContext);
					gameComponentsWorker.Save(code, game);
					if (gameComponentsWorker.Messages.Count() != 0)
					{
						foreach (var message in gameComponentsWorker.Messages)
						{
							logger.LogInformation("ParseCodeGameMessage: " + message);
						}
						return View("Create");
					}
					else
					{
						game.PathCode = ".." + pathCode;

						dataContext.Games.Add(game);
						dataContext.SaveChanges();
						return Redirect("~/Home/Index");
					}
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

		#region Play game
		public static class WayControler
		{
			private static Dictionary<int, int> percents;
			private static Dictionary<int, int> percentsLastUsed; // for remamber intevals
			private static SortedSet<int> keys;

			public static int[] WayControl(SortedSet<int> _keys, Transition[] transitions)
			{
				percents = new Dictionary<int, int>();
				percentsLastUsed = new Dictionary<int, int>();
				keys = _keys;

				var result = new Queue<int>();
				for (int i = 0; i < transitions.Length; ++i)
				{
					if (transitions[i].Condition.Length == 0 || IsCorrectWay(transitions[i].Condition))
					{
						result.Enqueue(i);
					}
				}

				return result.ToArray();
			}

			private static bool IsCorrectWay(string booleanExp)
			{
				var random = new Random(DateTime.Now.Millisecond);
				var stack = new Stack<bool>();
				var expression = booleanExp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var item in expression)
				{
					if (item[0] == '&')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 & b2);
						continue;
					}
					if (item[0] == '|')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 | b2);
						continue;
					}
					if (item[0] == '=')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 == b2);
						continue;
					}
					if (item[0] == '-')
					{
						stack.Push(!stack.Pop());
						continue;
					}
					if (item[0] == '^')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 ^ b2);
						continue;
					}
					if (item.Last() == '%')
					{

						var temp = item.Substring(0, item.Length - 1);
						var numbers = temp.Split('_');
						int index = Int32.Parse(numbers[0]);
						int startInterval = 0;
						int endInterval = Int32.Parse(numbers[1]) + startInterval;
						int x;
						if (!percents.ContainsKey(index))
						{
							x = random.Next(1, 100);
							percents.Add(index, x);
							percentsLastUsed.Add(index, endInterval);
						}
						else
						{
							x = percents[index];
							startInterval = percentsLastUsed[index];
							endInterval += startInterval;
							percentsLastUsed.Remove(index);
							percentsLastUsed.Add(index, endInterval);
						}
						stack.Push(x > startInterval && x <= endInterval);
						continue;
					}

					int key = Int32.Parse(item);
					stack.Push(keys.Contains(key));
				}
				return stack.Pop();
			}
		}

		[HttpGet]
		public IActionResult Play(string gameIdEncode)
		{
			logger.LogInformation("Visit /Game/Play page");
			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			if (HttpContext.User.Identity.IsAuthenticated)
			{
				int userID = Int32.Parse(HttpContext.User.Identity.Name);
				var save = dataContext.SavedGames.FirstOrDefault(i => i.GameId == gameId &&
					i.UserId == userID);

				if (save != null)
				{
					var splitedKeys = save.Keys.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					var keys = new SortedSet<int>();
					foreach (var key in splitedKeys) // filling keys
					{
						keys.Add(Int32.Parse(key));
					}

					model = new PlayModel()
					{
						GameID = gameId,
						Keys = save.Keys,
						Point = dataContext.PointDialogs.FirstOrDefault(i => i.GameId == gameId &&
							i.StateNumber == save.State),
						Transitions = new List<Transition>()
					};

					UpdateTransition(keys);
					return View(model);
				}
				else
				{
					save = new SavedGame
					{
						UserId = userID,
						GameId = gameId,
						State = 1,
						Keys = ""
					};
					dataContext.SavedGames.Add(save);
					dataContext.SaveChanges();
				}
			}

			model = new PlayModel()
			{
				GameID = gameId,
				Keys = "",
				Point = dataContext.PointDialogs.FirstOrDefault(i => i.GameId == gameId &&
					i.StateNumber == 1),
				Transitions = new List<Transition>()
			};

			UpdateTransition(new SortedSet<int>());
			return View(model);
		}

		[HttpPost]
		public IActionResult Answer(string keys_transitionEncode, string id_transitionEncode)
		{
			string _keys = ProtectData.GetInstance().DecodeToString(keys_transitionEncode);
			var splitedKeys = _keys.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var keys = new SortedSet<int>();
			foreach (var key in splitedKeys) // filling keys
			{
				keys.Add(Int32.Parse(key));
			}

			var id_transitionDecode = ProtectData.GetInstance().DecodeToString(id_transitionEncode);
			var id_transition = Int32.Parse(id_transitionDecode);
			var transition = dataContext.Transitions.FirstOrDefault(i => i.Id == id_transition);

			string newKey = UpdateKeys(keys, transition);
			model = new PlayModel()
			{
				GameID = transition.GameId,
				Keys = newKey,
				Point = dataContext.PointDialogs.FirstOrDefault(i => i.GameId == transition.GameId &&
					i.StateNumber == transition.NextPoint),
				Transitions = new List<Transition>()
			};
			UpdateTransition(keys);
			// if autorization
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				int userID = Int32.Parse(HttpContext.User.Identity.Name);
				var save = dataContext.SavedGames.FirstOrDefault(i => i.GameId == model.Point.GameId &&
					i.UserId == userID);

				save.Keys = model.Keys;
				save.State = model.Point.StateNumber;
				dataContext.Attach(save).State = EntityState.Modified;
				dataContext.SaveChanges();
			}
			return View("Play", model);
		}

		private void UpdateTransition(SortedSet<int> keys)
		{
			var transitions = dataContext.Transitions.Where(i => i.GameId == model.GameID &&
				i.StartPoint == model.Point.StateNumber).ToArray();

			var indexes = WayControler.WayControl(keys, transitions);
			foreach (int index in indexes)
			{
				model.Transitions.Add(transitions[index]);
			}
		}

		private string UpdateKeys(SortedSet<int> keys, Transition transiton)
		{
			var actions = dataContext.TransitionActions.Where(i => i.TransitionId == transiton.Id).ToArray();

			for (int i = 0; i < actions.Length; i++)
			{
				int key = actions[i].KeyAction;
				if (keys.Contains(key))
				{
					if (!actions[i].Type)
					{
						keys.Remove(key);
					}
				}
				else
				{
					if (actions[i].Type)
					{
						keys.Add(key);
					}
				}
			}

			string newKey = "";
			foreach (int key in keys)
			{
				newKey += key.ToString() + " ";
			}
			return newKey;
		}
		#endregion

		[HttpPost]
		public IActionResult Delete(string gameIdEncode)
		{
			logger.LogInformation("Visit /Game/Delete action");

			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			var gameDbWriter = new GameComponentsWorker(dataContext);
			//gameDbWriter.Delete(gameId);

			return Redirect("~/Account/Profile");
		}
	}
}