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
using System.IO;

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

		private List<IndexModel.GameInfo> Get_Games(IndexModel.TypeSelect typeSelect)
		{
			switch(typeSelect)
			{
				case IndexModel.TypeSelect.Last:
					return Enumerable.Reverse(dataContext.Games).Take(6).Select(i => new IndexModel.GameInfo
					{
						Id = i.Id,
						Title = i.Title,
						Rating = i.Rating,
						PathIcon = i.PathIcon
					}).ToList();
				case IndexModel.TypeSelect.Best:
					return dataContext.Games.OrderByDescending(i => i.Rating).Take(6).Select(i => new IndexModel.GameInfo
					{
						Id = i.Id,
						Title = i.Title,
						Rating = i.Rating,
						PathIcon = i.PathIcon
					}).ToList();
			}
			return dataContext.Games.Select(i => new IndexModel.GameInfo
			{
				Id = i.Id,
				Title = i.Title,
				Rating = i.Rating,
				PathIcon = i.PathIcon
			}).ToList();
		}

		[HttpPost]
		public IActionResult ChangeTypeSelect(IndexModel.TypeSelect typeSelect)
		{
			logger.LogInformation("Index page: Change type select game");
			
			var indexModel = new IndexModel
			{
				CurrentTypeSelect = typeSelect,
				Games = Get_Games(typeSelect)
			};
			return View("Index", indexModel);
		}

		[HttpGet]
		public IActionResult Index()
		{

			logger.LogInformation("Visit index page");
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				logger.LogInformation($"User {HttpContext.User.Identity.Name} is authorized");
			}

			var indexModel = new IndexModel
			{
				CurrentTypeSelect = IndexModel.TypeSelect.Last,
				Games = Get_Games(IndexModel.TypeSelect.Last)
			};
			return View(indexModel);
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
