using System;
using System.Collections.Generic;
using System.Linq;

//using GameInterpreror;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Data.GameComponents;
using GameTextParsing;

namespace WebVisualGame_MVC.Utilities
{

	public class GameComponentsWorker
	{
		private readonly DataContext dataContext;

		private readonly GameTextParser parser;

		public GameComponentsWorker(DataContext _dataContext)
		{
			parser = new GameTextParser();
			dataContext = _dataContext;
		}

		public MessageBuffer Messages { get; set; }

		public void Save(string sourceCode, Game gameNote)
		{
			//var resultParse
			if(parser.ParseGameText(sourceCode))
			{
				gameNote.PointDialogues = new List<PointDialog>();
				gameNote.Transitions = new List<Transition>();

				var gameData = parser.ProcessParseTree();

				if (gameData != null)
				{
					if (gameData.DialogPoints != null)
					{
						foreach (var dialogPoint in gameData.DialogPoints)
						{
							var dialogPointNote = new PointDialog
							{
								Background_imageURL = "",
								StateNumber = dialogPoint.Value.ID,
								Text = dialogPoint.Value.Text
							};
							gameNote.PointDialogues.Add(dialogPointNote);

							if (dialogPoint.Value.Links != null)
							{
								foreach (var dialogLink in dialogPoint.Value.Links)
								{
									var dialogLinkNote = new Transition
									{
										StartPoint = dialogPoint.Value.ID,
										NextPoint = dialogLink.NextID,
										Text = dialogLink.Text,
										Condition = dialogLink.Condition,
										TransitionActions = new List<TransitionAction>()
									};
									if (dialogLink.Actions != null)
									{
										foreach (var dialogLinkAction in dialogLink.Actions)
										{
											var dialogLinkActionNote = new TransitionAction
											{
												KeyAction = dialogLinkAction.Key,
												Type = dialogLinkAction.Action == ActionType.Find
											};
											dialogLinkNote.TransitionActions.Add(dialogLinkActionNote);
										}
									}
									gameNote.Transitions.Add(dialogLinkNote);
								}

							}
						}
					}
				}
			}
			Messages = parser.Messages;
		}

		public void Update(string sourceCode, Game gameNote)
		{
			//dataBase.Transitions.RemoveRange(dataBase.Transitions.Where(c => c.GameId == gameNote.Id));

			//dataBase.PointDialogs.RemoveRange(dataBase.PointDialogs.Where(c => c.GameId == gameNote.Id));

			//SaveGameComponents(sourceCode, gameNote);

			//try
			//{
			//	dataBase.Update(gameNote);

			//	dataBase.SaveChanges();
			//}
			//catch (Exception e)
			//{
			//	//...
			//}
		}

		public void Delete(Game gameNote)
		{
		//	dataBase.Transitions.RemoveRange(dataBase.Transitions.Where(c => c.GameId == gameNote.Id));

		//	dataBase.PointDialogs.RemoveRange(dataBase.PointDialogs.Where(c => c.GameId == gameNote.Id));

		//	//dataBase.Games.RemoveRange(dataBase.Games.FirstOrDefault(c => c.Id == gameId));

		//	dataBase.SaveChanges();
		}
	}
}

