using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebVisualGame_MVC.Models.PageModels.AccountModel;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;

namespace WebVisualGame_MVC.Controllers
{
    public class AccountController : Controller
    {
		private readonly DataContext dataContext;

		private readonly ILogger logger;

		private readonly IHostingEnvironment appEnvironment;

		public AccountController(DataContext _dataContext,
			IDataProtectionProvider provider,
			ILogger<AccountController> _logger,
			IHostingEnvironment _appEnvironment)
		{
			ProtectData.GetInstance().Initialize(provider);
			dataContext = _dataContext;
			appEnvironment = _appEnvironment;
			logger = _logger;
		}

		#region Authorization and Registration
		[HttpGet]
		public IActionResult Registration()
		{
			logger.LogInformation("Visit /Account/Registration page");
			return View(new RegistrationModel());
		}

		[HttpPost]
		public async Task<IActionResult> Registration(RegistrationModel model)
		{
			if (ModelState.IsValid)
			{
				if (dataContext.Users.FirstOrDefault(i => i.Login == model.userInfo.Login) != null)
				{
					model.Errors.Add("Пользователь с таким логином уже существует");
					ModelState.AddModelError("", "Пользователь с таким логином уже существует");
				}
					
				if (dataContext.Users.FirstOrDefault(i => i.Email == model.userInfo.Email) != null)
				{
					model.Errors.Add("Пользователь с таким адресом эл.почты уже существует");
					ModelState.AddModelError("", "Пользователь с таким адресом эл.почты уже существует");
				}	

				if (!ModelState.IsValid)
				{
					return View(model);
				}
				else
				{
					var user = new User
					{
						FirstName = model.userInfo.FirstName,
						LastName = model.userInfo.LastName,
						Login = model.userInfo.Login,
						Email = model.userInfo.Email,
						Password = model.userInfo.Password,
						PathAvatar = "../images/user/default_avatar.ico",
						Date = DateTime.Now
					};

					try
					{
						dataContext.Users.Add(user);
						dataContext.SaveChanges();
					}
					catch (Exception ex)
					{
						logger.LogError($"{ex.Message}");

						// user will see error page
						throw ex;
					}

					var userId = dataContext.Users.FirstOrDefault(i => i.Login == user.Login).Id;

					await Authorize(userId.ToString()); // авторизация

					return Redirect("~/Home/Index");
				}
			}
			return View(model);		
		}

		[HttpGet]
		public IActionResult Authorization()
		{
			logger.LogInformation("Visit /Account/Authorization page");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Authorization(AuthorizationModel model)
		{
			logger.LogInformation($"Began authorization for user '{model.Login}'");

			if (ModelState.IsValid)
			{
				var user = dataContext.Users.FirstOrDefault(i => i.Login == model.Login && i.Password == model.Password);
				if (user != null)
				{
					await Authorize(user.Id.ToString()); // авторизация

					logger.LogInformation($"User has authorized");

					return Redirect("~/Home/Index");
				}

				logger.LogInformation($"Authorization hasn't been passed");
				ViewBag.Error = "Некорректные логин и(или) пароль";
				ModelState.AddModelError("", "Некорректные логин и(или) пароль");
			}
			return View(model);
		}

		private async Task Authorize(string DB_id)
		{
			var claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, DB_id)
				};

			ClaimsIdentity id = new ClaimsIdentity(
				claims,
				"ApplicationCoockie",
				ClaimsIdentity.DefaultNameClaimType,
				ClaimsIdentity.DefaultRoleClaimType
				);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
		}
		#endregion

		#region Редактирование профиля
		[Authorize]
		[HttpGet]
		public IActionResult Redaction()
		{
			logger.LogInformation("Visit /Account/Redaction page");

			var userId = Int32.Parse(HttpContext.User.Identity.Name);
			var user = dataContext.Users.Single(i => i.Id == userId);

			var model = new RedactionModel();
			model.userInfo = new RedactionModel.UserInfo()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Login = user.Login,
				Password = user.Password
			};
			
			return View(model);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Redaction(RedactionModel model)
		{
			if (ModelState.IsValid)
			{
				var userId = Int32.Parse(HttpContext.User.Identity.Name);
				var user = dataContext.Users.Single(i => i.Id == userId);

				if (dataContext.Users.FirstOrDefault(i => i.Login == model.userInfo.Login) != null 
					&& user.Login != model.userInfo.Login)
				{
					model.Errors.Add("Пользователь с таким логином уже существует");
					ModelState.AddModelError("", "Пользователь с таким логином уже существует");
				}

				if (dataContext.Users.FirstOrDefault(i => i.Email == model.userInfo.Email) != null
					&& user.Email != model.userInfo.Email)
				{
					model.Errors.Add("Пользователь с таким адресом эл.почты уже существует");
					ModelState.AddModelError("", "Пользователь с таким адресом эл.почты уже существует");
				}

				if (!ModelState.IsValid)
				{
					return View(model);
				}
				else
				{
					user.FirstName = model.userInfo.FirstName;
					user.LastName = model.userInfo.LastName;
					user.Login = model.userInfo.Login;
					user.Email = model.userInfo.Email;
					user.Password = model.userInfo.Password;

					if (model.userInfo.Avatar != null)
					{
						// путь к папке /images/user/
						string pathAvatar = "/images/user/" + model.userInfo.Avatar.FileName;
						// сохраняем файл в папку /images/user/ в каталоге wwwroot
						using (var fileStream = new FileStream(appEnvironment.WebRootPath + pathAvatar, FileMode.Create))
						{
							await model.userInfo.Avatar.CopyToAsync(fileStream);
						}
						user.PathAvatar = ".." + pathAvatar;
					}

					try
					{
						dataContext.Attach(user).State = EntityState.Modified;
						dataContext.SaveChanges();
					}
					catch (Exception ex)
					{
						logger.LogError($"{ex.Message}");

						// user will see error page
						throw ex;
					}

					return Redirect("~/Account/Profile");
				}
			}
			return View(model);
		}
		#endregion

		[Authorize]
		[HttpGet]
		public IActionResult Profile()
		{
			logger.LogInformation("Visit /Account/Profile page");

			try
			{
				var userId = Int32.Parse(HttpContext.User.Identity.Name);
				var user = dataContext.Users.Single(i => i.Id == userId);

				var profileModel = new ProfileModel()
				{
					UserName = user.FirstName + ' ' + user.LastName,
					Email = user.Email,
					PathUserAvatar = user.PathAvatar,
					Data = user.Date,
					AccessLevel = user.AccessLevel
				};
				
				profileModel.UserGames = (from game in dataContext.Games.Where(i => i.UserId == userId)
										  select new ProfileModel.UserGame
										  {
											  Id = game.Id,
											  Title = game.Title,
											  Rating = game.Rating,
											  PathIcon = game.PathIcon
										  }).ToList();
				return View(profileModel);
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw ex;
			}
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect("~/Home/Index");
		}
	}
}