using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame_MVC.Models.PageModels.GameModel
{
	public class CreateModel
	{
		[Required(ErrorMessage = "Введите название игры")]
		public string Title { get; set; }

		public string Description { get; set; }

		public IFormFile Icon { get; set; }

		[Required(ErrorMessage = "Прикрепите код игры")]
		public IFormFile Code { get; set; }
	}
}
