using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data
{
	public class User
	{
		public int Id { get; set; }
		public string ActiveKey { get; set; }
		[Required(ErrorMessage = "Укажите Ваше имя")]
		public string Login { get; set; }
		[Required(ErrorMessage = "Укажите Ваш логин")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Укажите Ваш пароль")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Укажите Вашу фамилию")]
		public string LastName { get; set; }
		[Required(ErrorMessage = "Укажите Ваш Email")]
		[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)"
		+ @"@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Укажите корректный Email, псина,пожалуйста")]
		public string Email { get; set; }
	}
}
