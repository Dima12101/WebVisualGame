using System;
using System.Collections.Generic;

using GameInterpreror;

using WebVisualGame.Data;

namespace WebVisualGame
{

    public class GameDbWriter
    {
        private readonly GameMachineReader reader;

        private readonly Repository dataBase;

        public GameDbWriter(Repository db)
        {
            reader = new GameMachineReader();

            dataBase = db;
        }

        public void SaveGameToDd(string path, int gameID)
        {
            Queue<DialogPoint> dpQueue = new Queue<DialogPoint>();

            HashSet<DialogPoint> dpSet = new HashSet<DialogPoint>();

            DialogPoint gameStart = null;

            try
            {
                gameStart = reader.ReadGame(path);
            }
            catch (ApplicationException e)
            {
                // handling exceptions
            }

            dpQueue.Enqueue(gameStart);

            dpSet.Add(gameStart);

            while (dpQueue.Count > 0)
            {
                DialogPoint currPoint = dpQueue.Dequeue();

                var pointDialogNote = new Data.GameData.PointDialog()
                {
                    Background_imageURL = "",
                    StateNumber = currPoint.ID,
                    Text = currPoint.Text,
                    GameId = gameID,
                    PointDialogActions = new List<Data.GameData.PointDialogAction>()
                };

				if (currPoint.Actions != null)
				{
					for (int i = 0; i < currPoint.Actions.Length; ++i)
					{
						var actionNote = new Data.GameData.PointDialogAction()
						{
							Type = (currPoint.Actions[i].Type == ActionType.FindKey),
							KeyAction = currPoint.Actions[i].KeyNumber,
						};

						pointDialogNote.PointDialogActions.Add(actionNote);
					}
				}
                try
                {
                    dataBase.PointDialogs.Add(pointDialogNote);
                }
                catch (Exception e)
                {
                    // handling db writing exception
                }


                foreach (var currLink in currPoint.Links)
                {
                    var linkNote = new Data.GameData.Transition()
                    {
                        StartPoint = currPoint.ID,
                        NextPoint = currLink.NextPoint.ID,
                        GameId = gameID,
                        Conditions = new List<Data.GameData.Condition>(),
                        TransitionActions = new List<Data.GameData.TransitionAction>(),
						Text = currLink.Text
                    };

                    for (int i = 0; i < currLink.Conditions.Length; ++i)
                    {
                        var conditionNote = new Data.GameData.Condition()
                        {
                            KeyСondition = currLink.Conditions[i].KeyNumber,
                            Type = (currLink.Conditions[i].Type == ConditionType.Have)
                        };

                        linkNote.Conditions.Add(conditionNote);
                    }

                    for (int i = 0; i < currLink.Actions.Length; ++i)
                    {
                        var linkActionNote = new Data.GameData.TransitionAction()
                        {
                            Type = (currLink.Actions[i].Type == ActionType.FindKey),
                            KeyAction = currLink.Actions[i].KeyNumber
                        };

                        linkNote.TransitionActions.Add(linkActionNote);
                    }

                    try
                    {
                        dataBase.Transitions.Add(linkNote);
                    }
                    catch (Exception e)
                    {
                        // 
                    }

                    if (!dpSet.Contains(currLink.NextPoint))
                    {
                        dpQueue.Enqueue(currLink.NextPoint);

                        dpSet.Add(currLink.NextPoint);
                    }
                }
            }
        }
    }
}
