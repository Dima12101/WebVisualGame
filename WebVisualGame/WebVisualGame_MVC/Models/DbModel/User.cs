using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebVisualGame_MVC.Data
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(100)]
		[Required]
		public string FirstName { get; set; }

		[MaxLength(100)]
		[Required]
		public string LastName { get; set; }

		[MaxLength(100)]
		[Required]
		public string Login { get; set; }

		[MaxLength(100)]
		[Required]
		public string Password { get; set; }

		[MaxLength(100)]
		[Required]
		public string Email { get; set; }

		[Required]
		public string PathAvatar { get; set; }

		[Range(0, 1, ErrorMessage = "Введите либо 0, либо 1")]
		[Required]
		public int AccessLevel { get; set; }

		public IList<Game> Games { get; set; }

		public IList<GameComponents.SavedGame> SavedGames { get; set; }

		public IList<GameComponents.Review> Reviews { get; set; }

	}
}
