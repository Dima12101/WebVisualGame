using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebVisualGame_MVC.Models.PageModels.AccountModel
{
	public class RegistrationModel
	{
		public RegistrationModel()
		{
			Errors = new List<string>();
		}

		public List<string> Errors { get; set; }


		[Required(ErrorMessage = "Не указано имя")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Не указана фамилия")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Не указан логин")]
		public string Login { get; set; }

		[Required(ErrorMessage = "Не указан Email")]
		[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)"
		+ @"@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Указан не корректный Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Не указан пароль")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Пароль введен неверно")]
		public string ConfirmPassword { get; set; }
	}
}
