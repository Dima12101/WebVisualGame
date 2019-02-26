using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
		public PointDialog Point { get; private set; }
		[BindProperty]
		public Transition[] Transitions { get; set; }

		public PlayingModel(Repository _db)
		{
			keys = new SortedSet<int>();
			db = _db;
		}

		public void OnGet()
		{
			gameId = int.Parse(Request.Cookies["GameID"]);

			var pointNumber = int.Parse(Request.Cookies["StartPoint"]);
			Point = db.PointDialogs.FirstOrDefault(i => i.GameId == gameId &&
			i.StateNumber == pointNumber);

			var stringKey = Request.Cookies["SetKeys"];
			var numCollection = Regex.Matches(stringKey, "[0-9]+");
			foreach (Match it in numCollection)
			{
				keys.Add(int.Parse(it.Value));
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

			Response.Cookies.Delete("SetKeys");
			Response.Cookies.Delete("StartPoint");
			string newKey = "";
			foreach (int key in keys)
			{
				newKey += " " + key.ToString();
			}
			Response.Cookies.Append("SetKeys", newKey);
			Response.Cookies.Append("StartPoint", Point.StateNumber.ToString());
			return RedirectToPage();
		}

		private void UpdateTransition()
		{
			var newTransitons = db.Transitions.Where(i => i.GameId == gameId &&
				i.StartPoint == Point.StateNumber).ToList();

			var mergedTables = (from trans in newTransitons
								join cond in db.Conditions on trans.Id equals cond.TransitionId
								orderby (trans.Id)
								select new
								{
									Id = trans.Id,
									Type = cond.Type,
									KeyCondition = cond.KeyСondition
								}).ToArray();
			// deleting impossible transitions
			for (int i = 0; i < mergedTables.Length; ++i)
			{
				// Type == 1 => keys canrains KeyÑondition, else deleting Transition
				// table have to been sorted
				if (mergedTables[i].Type !=
					keys.Contains(mergedTables[i].KeyCondition))
				{
					int transitionId = mergedTables[i].Id;
					FilterTransition(newTransitons, transitionId);
					while (i < mergedTables.Length && mergedTables[i].Id == transitionId)
					{
						++i;
					}
					--i;
				}
			}
			Transitions = new Transition[newTransitons.Count];

			int j = 0;
			foreach (var newTransition in newTransitons)
			{
				Transitions[j] = newTransition;
				++j;
			}
		}

		private void FilterTransition(List<Transition> transitions, int id)
		{
			foreach (var transition in transitions)
			{
				if (transition.Id == id)
				{
					transitions.Remove(transition);
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
	}
}