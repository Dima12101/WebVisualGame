using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame.Data
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Укажите Ваше имя")]
		public string FirstName { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Укажите Вашу фамилию")]
		public string LastName { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Укажите Ваш логин")]
		public string Login { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Укажите Ваш пароль")]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		public string PasswordConfirm { get; set; }

		[MaxLength(100)]
		[Required(ErrorMessage = "Укажите Ваш Email")]
		[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)"
		+ @"@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Укажите корректный Email, псина,пожалуйста")]
		public string Email { get; set; }

		public DbSet<Game> games { get; set; }
	}
}
