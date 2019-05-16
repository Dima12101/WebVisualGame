using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

		private async Task<string> CreateFileCode_onServer(IFormFile Code, string pathCode)
		{
			//Path: /images/game/{nameDir}/{nameFile}

			// сохраняем файл в папку /files/gameCode/ в каталоге wwwroot
			using (var fileStream = new FileStream(appEnvironment.WebRootPath + pathCode, FileMode.Create))
			{
				await Code.CopyToAsync(fileStream);
			}
			string codeStr = "";
			// считываем переданный файл в string
			using (var reader = new StreamReader(Code.OpenReadStream(), detectEncodingFromByteOrderMarks: true))
			{
				codeStr = await reader.ReadToEndAsync();
			}
			return codeStr;
		}
		private async void CreateFileImage_onServer(IFormFile Image, string pathImage)
		{
			//Path: /images/game/{nameDir}/{nameFile}

			// сохраняем файл в папку /files/gameCode/ в каталоге wwwroot
			using (var fileStream = new FileStream(appEnvironment.WebRootPath + pathImage, FileMode.Create))
			{
				await Image.CopyToAsync(fileStream);
			}
		}
		private void DeleteDirictory(string path)
		{
			//Path: /images/game/{nameDir}
			DirectoryInfo dirInfo = new DirectoryInfo(appEnvironment.WebRootPath + path);
			foreach (FileInfo file in dirInfo.GetFiles())
			{
				file.Delete();
			}
			Directory.Delete(appEnvironment.WebRootPath + path);
		}
		private void DeleteFile(string path)
		{
			//Path: /images/game/{nameDir}/{nameFile}
			var fileIcon = new FileInfo(appEnvironment.WebRootPath + path);
			if (fileIcon != null)
			{
				fileIcon.Delete();
			}
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
					Rating = game.Rating,
					Data = game.Date
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
		public IActionResult Create(CreateModel model)
		{
			if (ModelState.IsValid)
			{
				logger.LogInformation($"Began create '{model.Title}'");
				var game = new Game();
				try
				{			
					game.UserId = Int32.Parse(HttpContext.User.Identity.Name);
					game.Title = model.Title;
					game.Description = model.Description;
					game.Rating = 0;
					game.PathIcon = "";
					game.PathCode = "";
					game.Date = DateTime.Now;

					//Для того, чтобы знать Id
					dataContext.Games.Add(game);
					dataContext.SaveChanges();

					string nameDirectory = $"{game.Title}_key{game.Id}";

					Directory.CreateDirectory($"./wwwroot/images/game/{nameDirectory}"); //с точкой!
					if (model.Icon != null)
					{
						string nameFileIcon = model.Icon.FileName.Split("\\").Last(); //В IE имя это путь
						// путь к папке /images/game/{nameDirectory}/{nameFileIcon}
						string pathIcon = $"/images/game/{nameDirectory}/{nameFileIcon}";

						CreateFileImage_onServer(model.Icon, pathIcon);

						game.PathIcon = ".." + pathIcon; //нужны 2 точки
					}
					else
					{
						game.PathIcon = "../images/game/NotImage.jpg"; //дефолт
					}

					Directory.CreateDirectory($"./wwwroot/files/gameCode/{nameDirectory}");
					string nameFileCode = model.Code.FileName.Split("\\").Last(); //В IE имя это путь
					// путь к папке /files/gameCode/{nameDirectory}/{nameFileCode}
					string pathCode = $"/files/gameCode/{nameDirectory}/{nameFileCode}"; 

					string code = CreateFileCode_onServer(model.Code, pathCode).Result;

					game.PathCode = ".." + pathCode; //нужны 2 точки

					// парсинг кода и сохранения в БД
					var gameComponentsControl = new GameComponentsControl(dataContext);
					gameComponentsControl.Save(code, game);

					if (gameComponentsControl.Messages.Count() != 0)
					{
						foreach (var message in gameComponentsControl.Messages)
						{
							model.Messages.Add(model.Messages.Count.ToString() + ": " + message);
							logger.LogInformation("ParseCodeGameMessage: " + message);
						}

						//Зачищаем папку и удаляем её
						var pathDirictory = $"/images/game/{game.Title}_key{game.Id}";
						DeleteDirictory(pathDirictory);

						pathDirictory = $"/files/gameCode/{game.Title}_key{game.Id}";
						DeleteDirictory(pathDirictory);

						dataContext.Games.Remove(game);
						dataContext.SaveChanges();

						return View("Create", model);
					}
					else
					{
						dataContext.Attach(game).State = EntityState.Modified;
						dataContext.SaveChanges();
						return Redirect("~/Home/Index");
					}
				}
				catch (Exception ex)
				{
					//Зачищаем папку и удаляем её
					var pathDirictory = $"/images/game/{game.Title}_key{game.Id}";
					DeleteDirictory(pathDirictory);

					pathDirictory = $"/files/gameCode/{game.Title}_key{game.Id}";
					DeleteDirictory(pathDirictory);

					dataContext.Games.Remove(game);
					dataContext.SaveChanges();

					logger.LogError(ex.Message);
					throw ex;
				}
			}
			return View(model);
		}
		#endregion

		#region Redact game
		private RedactModel Get_redactModel(int gameId)
		{
			var game = dataContext.Games.Single(i => i.Id == gameId);

			var model = new RedactModel();

			model.GameId = gameId;

			model.PathIcon = game.PathIcon;
			model.baseInfo = new RedactModel.BaseInfo
			{
				Title = game.Title,
				Description = game.Description
			};

			var pathCode = appEnvironment.WebRootPath + game.PathCode.Replace("..", "");
			model.codeInfo = new RedactModel.CodeInfo
			{
				Messages = new List<string>(),
				CodeText = System.IO.File.ReadAllText(pathCode, Encoding.Default),
				CodeFile = null
			};

			var images = dataContext.Images.Where(i => i.GameId == gameId);
			model.bindImages = new List<RedactModel.BindImage>();
			foreach (var image in images)
			{
				model.bindImages.Add(new RedactModel.BindImage
				{
					Name = $"{image.Name.Replace($"_key{gameId}", "")}",
					File = null,
					Path = image.Path
				});
			}
			return model;
		}

		[HttpGet]
		public IActionResult Redact(string gameIdEncode)
		{
			logger.LogInformation("Visit /Game/Redact page");

			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);
			logger.LogInformation("GameId: " + gameIdDecoded);

			return View(Get_redactModel(gameId));
		}

		[HttpPost]
		public IActionResult ChangeBaseInfo(RedactModel.BaseInfo baseInfo, string gameIdEncode)
		{
			logger.LogInformation("Redact baseInfo");
			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			var game = dataContext.Games.Single(i => i.Id == gameId);

			//Из-за смены имени игры идёт смена директории
			string OldNameDirectory = $"{game.Title}_key{game.Id}";
			string NewNameDirectory = $"{baseInfo.Title}_key{game.Id}";
			if(OldNameDirectory != NewNameDirectory)
			{
				//Перемещение папки с изображениями
				Directory.Move($"{appEnvironment.WebRootPath}/images/game/{OldNameDirectory}", $"{appEnvironment.WebRootPath}/images/game/{NewNameDirectory}");
				string nameFileImage = game.PathIcon.Split("/").Last();
				game.PathIcon = $"../images/game/{NewNameDirectory}/{nameFileImage}";
				var bindImages = dataContext.Images.Where(i => i.GameId == gameId);
				foreach(var bindImage in bindImages)
				{
					bindImage.Path = $"../images/game/{NewNameDirectory}/{bindImage.Name}.jpg";
					dataContext.Attach(bindImage).State = EntityState.Modified;
				}

				//Перемещение папки с кодом
				Directory.Move($"{appEnvironment.WebRootPath}/files/gameCode/{OldNameDirectory}", $"{appEnvironment.WebRootPath}/files/gameCode/{NewNameDirectory}");
				string nameFileCode = game.PathCode.Split("/").Last();
				game.PathCode = $"../files/gameCode/{NewNameDirectory}/{nameFileCode}";
			}

			if (baseInfo.Icon != null)
			{
				//Удалям старую иконку
				string nameOldIcon = game.PathIcon.Split("/").Last();
				string pathOldIcon = $"/images/game/{NewNameDirectory}/{nameOldIcon}";
				DeleteFile(pathOldIcon);

				//Размещаем новую иконку
				string nameNewIcon = baseInfo.Icon.FileName.Split("\\").Last();
				string pathNewIcon = $"/images/game/{NewNameDirectory}/{nameNewIcon}";
				CreateFileImage_onServer(baseInfo.Icon, pathNewIcon);

				game.PathIcon = ".." + pathNewIcon;
			}

			game.Title = baseInfo.Title;
			game.Description = baseInfo.Description;

			dataContext.Attach(game).State = EntityState.Modified;
			dataContext.SaveChanges();

			return View("Redact", Get_redactModel(gameId));
		}

		[HttpPost]
		public IActionResult ChangeCode(RedactModel.CodeInfo codeInfo, string gameIdEncode)
		{
			logger.LogInformation("Redact code");
			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			var game = dataContext.Games.Single(i => i.Id == gameId);

			//Если был загружен файл, то применён будет он, иначе код из формы
			if (codeInfo.CodeFile != null)
			{
				//Путь к имеющемуся коду
				var OldPath_FileCode = game.PathCode.Replace("..", "");

				//Путь к поступившему коду
				var NewName_FileCode = codeInfo.CodeFile.FileName.Split("\\").Last();
				var NewPath_FileCode = $"/files/gameCode/{game.Title}_key{game.Id}/{NewName_FileCode}"; 
				var newCode = CreateFileCode_onServer(codeInfo.CodeFile, NewPath_FileCode).Result;

				// парсинг кода и сохранения в БД
				var gameComponentsControl = new GameComponentsControl(dataContext);
				gameComponentsControl.Update(newCode, game);

				if (gameComponentsControl.Messages.Count() != 0)
				{
					var model = Get_redactModel(gameId);
					foreach (var message in gameComponentsControl.Messages)
					{
						model.codeInfo.Messages.Add(model.codeInfo.Messages.Count.ToString() + ": " + message);
						logger.LogInformation("ParseCodeGameMessage: " + message);
					}
					DeleteFile(NewPath_FileCode);
					View("Redact", model);
				}
				else
				{
					//Если новый файл был того же имени, то старый файл был перезаписан
					if(OldPath_FileCode != NewPath_FileCode)
					{
						game.PathCode = ".." + NewPath_FileCode;
						DeleteFile(OldPath_FileCode);
					}
				}
			}
			else
			{
				var newCode = codeInfo.CodeText;

				// парсинг кода и сохранения в БД
				var gameComponentsControl = new GameComponentsControl(dataContext);
				gameComponentsControl.Update(newCode, game);

				if (gameComponentsControl.Messages.Count() != 0)
				{
					var model = Get_redactModel(gameId);
					foreach (var message in gameComponentsControl.Messages)
					{
						model.codeInfo.Messages.Add(model.codeInfo.Messages.Count.ToString() + ": " + message);
						logger.LogInformation("ParseCodeGameMessage: " + message);
					}
					return View("Redact", model);
				}
				else
				{
					System.IO.File.WriteAllText(appEnvironment.WebRootPath + game.PathCode.Replace("..", ""), newCode);
				}
			}

			dataContext.Attach(game).State = EntityState.Modified;
			dataContext.SaveChanges();

			return View("Redact", Get_redactModel(gameId));
		}

		[HttpPost]
		public IActionResult ChangeBindingImages(List<RedactModel.BindImage> bindImages, string gameIdEncode)
		{
			logger.LogInformation("Redact bindImages");
			var gameIdDecoded = ProtectData.GetInstance().DecodeToString(gameIdEncode);
			var gameId = Int32.Parse(gameIdDecoded);

			var game = dataContext.Games.Single(i => i.Id == gameId);

			foreach (var bindImage in bindImages)
			{
				if (bindImage.File != null)
				{
					var nameBindImage = $"{bindImage.Name}_key{gameId}";
					var image = dataContext.Images.Single(im => im.GameId == gameId && im.Name == nameBindImage);

					var pathBindImage = $"/images/game/{game.Title}_key{game.Id}/{bindImage.Name}_key{game.Id}.jpg";
					DeleteFile(pathBindImage);
					CreateFileImage_onServer(bindImage.File, pathBindImage);

					image.Path = ".." + pathBindImage;
					dataContext.Attach(image).State = EntityState.Modified;
				}
			}
			dataContext.SaveChanges();

			return View("Redact", Get_redactModel(gameId));
		}
		#endregion

		#region Play game
		private static class WayControler
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
			try
			{
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
					Transitions = new List<Transition>(),
					PathImage = "../images/game/NotImage.jpg"
				};
				if (model.Point.ImageId != null)
				{
					model.PathImage = dataContext.Images.SingleOrDefault(i => i.Id == model.Point.ImageId).Path;
				}
				
				

				UpdateTransition(new SortedSet<int>());
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw ex;
			}

			return View(model);
		}

		[HttpPost]
		public IActionResult Answer(string keys_transitionEncode, string id_transitionEncode)
		{
			try
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
					Transitions = new List<Transition>(),
					PathImage = "../images/game/NotImage.jpg"
				};
				if (model.Point.ImageId != null)
				{
					model.PathImage = dataContext.Images.SingleOrDefault(i => i.Id == model.Point.ImageId).Path;
				}
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
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw ex;
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

			var gameComponentsControl = new GameComponentsControl(dataContext);
			gameComponentsControl.Delete(gameId);

			var game = dataContext.Games.Single(i => i.Id == gameId);

			//Зачищаем папку и удаляем её
			var pathDirictory = $"/images/game/{game.Title}_key{game.Id}";
			DeleteDirictory(pathDirictory);

			pathDirictory = $"/files/gameCode/{game.Title}_key{game.Id}";
			DeleteDirictory(pathDirictory);

			dataContext.Games.Remove(game);

			dataContext.SaveChanges();

			return Redirect("~/Account/Profile");
		}
	}
}