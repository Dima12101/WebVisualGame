using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebVisualGame_MVC.Models;
using WebVisualGame_MVC.Models.DbModel;
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
			logger.LogInformation("Test Message");
			var indexModel = new IndexModel(dataContext);

			if (Request.Cookies.ContainsKey("Login") && Request.Cookies.ContainsKey("Sign"))
			{
				var login = Request.Cookies["Login"];
				var sign = Request.Cookies["Sign"];
				if (sign == SignGenerator.GetSign(login + "bytepp"))
				{
					var userId =  Int32.Parse(Request.Cookies["UserId"]);
					indexModel.SetUser(userId);
				}
			}
			return View(indexModel);
		}

		[HttpPost]
		public IActionResult Exit()
		{
			Response.Cookies.Delete("UserId");
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("Sign");
			return RedirectPermanent("~/Home/Index");
		}




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
