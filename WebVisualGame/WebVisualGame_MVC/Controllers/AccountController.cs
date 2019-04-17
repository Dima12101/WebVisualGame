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
using WebVisualGame_MVC.Models.PageModels;

namespace WebVisualGame_MVC.Controllers
{
    public class AccountController : Controller
    {
		private DataContext dataContext;

		private ILogger logger;

		public AccountController(DataContext _dataContext, ILogger<AccountController> _logger)
		{
			dataContext = _dataContext;

			logger = _logger;
		}

		[HttpGet]
		public IActionResult Registration()
		{
			return View(new RegistrationModel());
		}

		[HttpPost]
		public async Task<IActionResult> Registration(RegistrationModel model)
		{
			if (ModelState.IsValid)
			{
				if (dataContext.Users.FirstOrDefault(i => i.Login == model.Login) != null)
				{
					model.Errors.Add("Пользователь с таким логином уже существует");
					ModelState.AddModelError("", "Пользователь с таким логином уже существует");
				}
					
				if (dataContext.Users.FirstOrDefault(i => i.Email == model.Email) != null)
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
						FirstName = model.FirstName,
						LastName = model.LastName,
						Login = model.Login,
						Email = model.Email,
						Password = model.Password
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

		public async Task Authorize(string DB_id)
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

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect("~/Home/Index");
		}
	}
}