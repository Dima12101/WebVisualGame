using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebVisualGame_MVC.Models;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Models.PageModels;
using WebVisualGame_MVC.Utilities;

namespace WebVisualGame_MVC.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger logger;

		private readonly DataContext dataContext;

		public HomeController(DataContext _dataContext, ILogger<HomeController> _logger, IDataProtectionProvider provider)
		{
			ProtectData.GetInstance().Initialize(provider);
			dataContext = _dataContext;
			logger = _logger;
		}

		public IActionResult Index()
		{
			logger.LogInformation("Visit index page");

			var indexModel = new IndexModel(dataContext);

			if (HttpContext.User.Identity.IsAuthenticated)
			{
				logger.LogInformation($"User {HttpContext.User.Identity.Name} is authorized");

				int userId = 0;

				logger.LogInformation($"Trying get user's id");

				try
				{
					userId = Int32.Parse(HttpContext.User.Identity.Name);
				}
				catch (Exception ex)
				{
					logger.LogError(ex.Message);

					throw ex;
				}

				logger.LogInformation($"User's id is {userId}");

				indexModel.SetUser(userId);
			}
			else
			{
				logger.LogInformation($"Authorization hasn't been passed");
			}

			return View(indexModel);
		}

		[Authorize]
		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
