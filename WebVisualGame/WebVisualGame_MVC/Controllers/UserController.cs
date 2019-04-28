using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Utilities;

namespace WebVisualGame_MVC.Controllers
{
    public class UserController : Controller
    {
		private readonly ILogger logger;

		private readonly DataContext dataContext;

		public UserController(DataContext _dataContext, ILogger<HomeController> _logger, IDataProtectionProvider provider)
		{
			ProtectData.GetInstance().Initialize(provider);
			dataContext = _dataContext;
			logger = _logger;
		}

		[HttpGet]
		[Authorize]
		public IActionResult Profile()
        {
            return View();
        }
    }
}