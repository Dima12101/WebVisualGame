using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVisualGame_MVC.Data.GameComponents;

namespace WebVisualGame_MVC.Models.PageModels.GameModel
{
	public class PlayModel
	{
		public PointDialog Point { get; set; }

		public string PathImage { get; set; }

		public List<Transition> Transitions { get; set; }

		public string Keys { get; set; }

		public int GameID { get; set; }

		//Для дебага (временно)
		public string LogState { get; set; }
	}
}
