using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebVisualGame.Data;
using WebVisualGame.Data.GameData;
using WebVisualGame.Utilities;

namespace WebVisualGame.Pages
{
    public class PlayingModel : PageModel
    {
		private readonly Repository db;
		private int gameId;
		private SortedSet<int> keys;

		public static class WayControler
		{
			private static Dictionary<int, int> percents;
			private static Dictionary<int, int> percentsLastUsed; // for remamber intevals
			private static SortedSet<int> keys;

			public static int[] WayControl(string _keys, string[] booleanExps)
			{
				percents = new Dictionary<int, int>();
				percentsLastUsed = new Dictionary<int, int>(); // for remamber intevals
				keys = new SortedSet<int>();
				var splitedKeys = _keys.Split();

				foreach (var key in splitedKeys)
				{
					keys.Add(Int32.Parse(key));
				}

				var result = new Queue<int>();
				for (int i = 0; i < booleanExps.Length; ++i)
				{
					if (IsCorrectWay(booleanExps[i]))
					{
						result.Enqueue(i);
					}
				}
				percents = null;
				percentsLastUsed = null;
				keys = null;
				return result.ToArray();
			}

			private static bool IsCorrectWay(string booleanExp)
			{
				var random = new Random(DateTime.Now.Millisecond);
				var stack = new Stack<bool>();
				var expression = booleanExp.Split();

				foreach (var item in expression)
				{
					if (item[0] == '&')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 & b2);
						continue;
					}
					if (item[0] == '|')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 | b2);
						continue;
					}
					if (item[0] == '=')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 == b2);
						continue;
					}
					if (item[0] == '~')
					{
						stack.Push(false == stack.Pop());
						continue;
					}
					if (item[0] == '^')
					{
						var b2 = stack.Pop();
						var b1 = stack.Pop();
						stack.Push(b1 ^ b2);
						continue;
					}
					if (item[item.Length - 1] == '%')
					{

						var temp = item.Substring(0, item.Length - 1);
						var numbers = temp.Split('_');
						int index = Int32.Parse(numbers[0]);
						int startInterval = 0;
						int endInterval = Int32.Parse(numbers[1]) + startInterval;
						int x;
						if (!percents.ContainsKey(index))
						{
							x = random.Next(1, 100);
							percents.Add(index, x);
							percentsLastUsed.Add(index, endInterval);
						}
						else
						{
							x = percents[index];
							startInterval = percentsLastUsed[index];
							endInterval += startInterval;
							percentsLastUsed.Remove(index);
							percentsLastUsed.Add(index, endInterval);
						}
						stack.Push(x > startInterval && x <= endInterval);
						continue;
					}

					int key = Int32.Parse(item);
					stack.Push(keys.Contains(key));
				}
				return stack.Pop();
			}
		}

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

		public IActionResult OnPostAnswer(string id_transition)
		{
			var id_transitionDecoded = ProtectData.GetInstance().DecodeToInt(id_transition);
			var transition = db.Transitions.FirstOrDefault(i => i.Id == id_transitionDecoded);
			Point = db.PointDialogs.FirstOrDefault(i => i.StateNumber == transition.NextPoint &&
			i.GameId == transition.GameId);

			var transiton_actions = db.TransitionActions.Where(i =>
				i.TransitionId == transition.Id).ToArray();
			var point_actions = db.PointDialogActions.Where(i =>
				i.PointDialogId == Point.Id).ToArray();

			if (Request.Cookies.ContainsKey("UserId")) 
			{
				SaveToDB(transiton_actions, point_actions);
			}
			else
			{
				SaveToCookies(transiton_actions, point_actions);
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

		private string FillingKey(string oldKey, TransitionAction[] transiton_actions,
			PointDialogAction[] point_actions)
		{
			var numCollection = Regex.Matches(oldKey, "[0-9]+");
			foreach (Match it in numCollection)
			{
				keys.Add(Int32.Parse(it.Value));
			}

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

			string newKey = "";
			foreach (int key in keys)
			{
				newKey += " " + key.ToString();
			}
			return newKey;
		}

		private void SaveToDB(TransitionAction[] transiton_actions,
			PointDialogAction[] point_actions)
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			var save = db.SavedGames.FirstOrDefault(i => i.GameId == Point.GameId &&
				i.UserId == userId);

			string key = FillingKey(save.Keys, transiton_actions, point_actions);

			save.Keys = key;
			save.State = Point.StateNumber;
			db.Attach(save).State = EntityState.Modified;
			db.SaveChanges();
		}

		private void SaveToCookies(TransitionAction[] transiton_actions,
			PointDialogAction[] point_actions)
		{
			string key = FillingKey(Request.Cookies["Keys"], transiton_actions, point_actions);
			Response.Cookies.Delete("Keys");
			Response.Cookies.Delete("Point");

			Response.Cookies.Append("Keys", key);
			Response.Cookies.Append("Point", Point.StateNumber.ToString());
		}

		private void GetFromDB()
		{
			int userId = Int32.Parse(Request.Cookies["UserId"]);
			int gameId = Int32.Parse(Request.Cookies["GameId"]);
			var save = db.SavedGames.FirstOrDefault(i => i.GameId == gameId &&
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

			string stringKey = Request.Cookies["Keys"];
			var numCollection = Regex.Matches(stringKey, "[0-9]+");
			foreach (Match it in numCollection)
			{
				keys.Add(int.Parse(it.Value));
			}
		}
	}
}