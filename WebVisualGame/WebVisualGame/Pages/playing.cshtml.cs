using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;
using WebVisualGame.Data.GameData;

namespace WebVisualGame.Pages
{
    public class PlayingModel : PageModel
    {
		private readonly Repository db;
		private int gameId;
		private SortedSet<int> keys;

		[BindProperty]
		public PointDialog Point { get; set; }
		[BindProperty]
		public List<Transition> Transitions { get; set; }

		public PlayingModel(Repository _db)
		{
			keys = new SortedSet<int>();
			db = _db;
		}

		public void OnGet()
		{
			gameId = int.Parse(Request.Cookies["GameID"]);
			if (Request.Cookies.ContainsKey("UserId"))
			{
				GetFromDB();
			}
			else
			{
				GetFromCookies();
			}
			UpdateTransition();
		}

		public IActionResult OnPostAnswer(int id_transition)
		{
			var transition = db.Transitions.FirstOrDefault(i => i.Id == id_transition);
			Point = db.PointDialogs.FirstOrDefault(i => i.StateNumber == transition.NextPoint &&
			i.GameId == transition.GameId);

			var transiton_actions = db.TransitionActions.Where(i =>
				i.TransitionId == transition.Id).ToArray();
			var point_actions = db.PointDialogActions.Where(i =>
				i.PointDialogId == Point.Id).ToArray();

			string stringKey = Request.Cookies["SetKeys"];
			var numCollection = Regex.Matches(stringKey, "[0-9]+");
			foreach (Match it in numCollection)
			{
				keys.Add(Int32.Parse(it.Value));
			}

			FillingKey(transiton_actions, point_actions);

			string newKey = "";
			foreach (int key in keys)
			{
				newKey += " " + key.ToString();
			}

			if (Request.Cookies.ContainsKey("UserId")) 
			{
				SaveToDB(newKey);
			}
			else
			{
				SaveToCookies(newKey);
			}
			return RedirectToPage();
		}

		private void UpdateTransition()
		{
			Transitions = db.Transitions.Where(i => i.GameId == gameId &&
				i.StartPoint == Point.StateNumber).ToList();

			var ConditionTables = (from trans in Transitions
								   join cond in db.Conditions on trans.Id equals cond.TransitionId
								orderby (trans.Id)
								select new
								{
									Id = trans.Id,
									Type = cond.Type,
									KeyCondition = cond.KeyСondition
								}).ToArray();
			// deleting impossible transitions
			for (int i = 0; i < ConditionTables.Length; ++i)
			{
				// Type == 1 => keys canrains KeyÑondition, else deleting Transition
				// table have to been sorted
				if (ConditionTables[i].Type !=
					keys.Contains(ConditionTables[i].KeyCondition))
				{
					int transitionId = ConditionTables[i].Id;
					FilterTransition(transitionId);
					while (i < ConditionTables.Length && ConditionTables[i].Id == transitionId)
					{
						++i;
					}
					--i;
				}
			}
		}

		private void FilterTransition(int id)
		{
			foreach (var transition in Transitions)
			{
				if (transition.Id == id)
				{
					Transitions.Remove(transition);
					return;
				}
			}
		}

		private void FillingKey(TransitionAction[] transiton_actions,
			PointDialogAction[] point_actions)
		{
			for (int i = 0; i < transiton_actions.Length; ++i)
			{
				if (transiton_actions[i].Type)
				{
					keys.Add(transiton_actions[i].KeyAction);
				}
				else
				{
					keys.Remove(transiton_actions[i].KeyAction);
				}
			}
			for (int i = 0; i < point_actions.Length; ++i)
			{
				if (point_actions[i].Type)
				{
					keys.Add(point_actions[i].KeyAction);
				}
				else
				{
					keys.Remove(point_actions[i].KeyAction);
				}
			}
		}

		private async void SaveToDB(string key)
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			var save = db.SavedGames.FirstOrDefault(i => i.GameId == Point.GameId &&
				i.UserId == userId);

			save.Keys = key;
			save.State = Point.StateNumber;
			db.Attach(save).State = EntityState.Modified;
			await db.SaveChangesAsync();
		}

		private void SaveToCookies(string key)
		{
			Response.Cookies.Delete("SetKeys");
			Response.Cookies.Delete("Point");

			Response.Cookies.Append("SetKeys", key);
			Response.Cookies.Append("Point", Point.StateNumber.ToString());
		}

		private void GetFromDB()
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			var save = db.SavedGames.FirstOrDefault(i => i.GameId == Point.GameId &&
				i.UserId == userId);
			Point = db.PointDialogs.FirstOrDefault(i => i.GameId == gameId &&
			i.StateNumber == save.State);

			string stringKey = save.Keys;
			var numCollection = Regex.Matches(stringKey, "[0-9]+");
			foreach (Match it in numCollection)
			{
				keys.Add(int.Parse(it.Value));
			}
		}

		private void GetFromCookies()
		{
			int pointNumber = int.Parse(Request.Cookies["Point"]);
			Point = db.PointDialogs.FirstOrDefault(i => i.GameId == gameId &&
			i.StateNumber == pointNumber);

			string stringKey = Request.Cookies["SetKeys"];
			var numCollection = Regex.Matches(stringKey, "[0-9]+");
			foreach (Match it in numCollection)
			{
				keys.Add(int.Parse(it.Value));
			}
		}
	}
}