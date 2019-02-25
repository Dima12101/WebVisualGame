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

                var pointDialogNote = new Data.GameData.PointDialogue()
                {
                    Background_imageURL = "",
                    StateNumber = currPoint.ID,
                    Text = currPoint.Text,
                    // temp stupid code
                    GameId = gameID
                };

                for (int i = 0; i < currPoint.Actions.Length; ++i)
                {
                    // creating actions

                    try
                    {
                        // adding actions
                        dataBase.PointDialogues.Add(pointDialogNote);
                    }
                    catch (Exception e)
                    {
                        // handling db writing exception
                    }
                }

                foreach (var currLink in currPoint.Links)
                {
                    var linkNote = new Data.GameData.Transition
                    {
                        StartPoint = currPoint.ID,
                        NextPoint = currLink.NextPoint.ID,
                        // temp stupid code
                        GameId = gameID,
                        NumberInList = 0
                    };

                    try
                    {
                        dataBase.Transitions.Add(linkNote);
                    }
                    catch (Exception e)
                    {

                    }

                    for (int i = 0; i < currLink.Conditions.Length; ++i)
                    {
                        var conditionNote = new Data.GameData.Сondition
                        {
                            // whaaaa?
                            //KeyСondition = currLink.Conditions[i].keyNumber,
                            //TransitionId = 0,
                        };

                        try
                        {
                            dataBase.Add(conditionNote);
                        }
                        catch (Exception e)
                        {
                            // handling writting exception
                        }

                    }

                    for (int i = 0; i < currLink.Actions.Length; ++i)
                    {
                        // creating action

                        try
                        {
                            // writting the action
                        }
                        catch (Exception e)
                        {
                            // db writting exception
                        }
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
