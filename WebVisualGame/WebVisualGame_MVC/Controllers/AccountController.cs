using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WebVisualGame_MVC.Models.DbModel;
using WebVisualGame_MVC.Utilities;

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
			return View();
		}

		[HttpPost]
		public IActionResult Registration(User user)
		{
			var serverErrors = new List<string>();
			if (dataContext.Users.FirstOrDefault(i => i.Login == user.Login) != null)
				serverErrors.Add("Пользователь с таким логином уже существует");
			if (dataContext.Users.FirstOrDefault(i => i.Email == user.Email) != null)
				serverErrors.Add("Пользователь с таким адресом эл.почты уже существует");

			if (serverErrors.Count != 0)
			{
				ViewBag.serverErrors = serverErrors;
				return View();
			}
			else
			{
				try
				{
					dataContext.Users.Add(null);
					dataContext.SaveChanges();
				}
				catch (Exception ex)
				{
					logger.LogError($"{ex.Message}");

					// user will see error page
					throw ex;
				}

				var userId = dataContext.Users.FirstOrDefault(i => i.Login == user.Login).Id;

				Response.Cookies.Append("UserId", userId.ToString());
				Response.Cookies.Append("Login", user.Login);
				Response.Cookies.Append("Sign", SignGenerator.GetSign(user.Login + "bytepp"));
				return Redirect("~/Home/Index");
			}
		}

		[HttpGet]
		public IActionResult Authorization()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Authorization(string login, string password)
		{
			var user = dataContext.Users.FirstOrDefault(i => i.Login == login && i.Password == password);
			if (user == null)
			{
				ViewBag.Error = "Введён неправильный логин или пароль";
				return View();
			}
			else
			{
				Response.Cookies.Append("UserId", user.Id.ToString());
				Response.Cookies.Append("Login", login);
				Response.Cookies.Append("Sign", SignGenerator.GetSign(login + "bytepp"));

				return Redirect("~/Home/Index");
			}
		}


	}
}