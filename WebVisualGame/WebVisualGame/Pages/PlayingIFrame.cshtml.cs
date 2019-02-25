using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Web;
using WebVisualGame.Data;
namespace WebVisualGame.Pages
{
    public class PlayingIFrameModel : PageModel
    {

		private readonly Repository db;
		private int gameID;
		private int pointID;
		private int[] matchingTransition;
		public String Text { get; private set; }

        public void OnGet(int id_transition)
        {
			id_transition = matchingTransition[id_transition];
			pointID = db.Transitions.FirstOrDefault(i => i.StartPoint == pointID &&
			i.GameId == gameID &&
			i.Id == id_transition).NextPoint;

			var matchingTransition = db.Transitions.Select(i => i.StartPoint == pointID &&
			i.GameId == gameID).ToArray();
			var l = db.Ñonditions.Select(i => i.TransitionId )
		}
    }
}