using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project_Infrastructure.Models;

namespace Project_Infrastructure.Controllers
{
    public class HomeController : Controller
    {
        
        // logger object for using in this class
        private readonly ILogger logger;

        // saving logger for continious using
        public HomeController(ILogger<HomeController> _logger)
        {
            logger = _logger;
            // logger example:
            logger.LogInformation("Created logger");
        }

        public IActionResult Index()
        {
            // logger example:
            logger.LogInformation("Got index page");

            return View();
        }

        public IActionResult About()
        {
            // logger example:
            logger.LogInformation("Got about page");

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            // logger example:
            logger.LogInformation("Got contact page");

            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            // logger example:
            logger.LogInformation("Got privacy page");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // logger example:
            logger.LogInformation("Got error page");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
