using System;
using System.Collections.Generic;
using System.Linq;

//using GameInterpreror;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Data.GameComponents;
using GameTextParsing;
using Microsoft.EntityFrameworkCore;

namespace WebVisualGame_MVC.Utilities
{

	public class GameComponentsControl
	{
		private readonly DataContext dataContext;

		private readonly GameTextParser parser;

		public GameComponentsControl(DataContext _dataContext)
		{
			parser = new GameTextParser();
			dataContext = _dataContext;
		}

		public MessageBuffer Messages { get; set; }

		public void Save(string sourceCode, Game gameNote)
		{
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
			Delete(gameNote.Id);
			Save(sourceCode, gameNote);
			dataContext.Attach(gameNote).State = EntityState.Modified;
		}

		public void Delete(int gameId)
		{
			dataContext.Transitions.RemoveRange(dataContext.Transitions.Where(c => c.GameId == gameId));

			dataContext.PointDialogs.RemoveRange(dataContext.PointDialogs.Where(c => c.GameId == gameId));
		}
	}
}

