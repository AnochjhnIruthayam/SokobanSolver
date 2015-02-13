using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanSolver
{
    class GameBoard
    {
        //Constructors
        public GameBoard() { }

        public GameBoard(string[,] _copyBoard, int _width, int _height)
        {
            board = _copyBoard;
            widthSize = _width;
            heightSize = _height;
        }

        public GameBoard(string[,] _copyBoard)
        {
            board = _copyBoard;
        }
        
        //property functions
        public int widthSize { get; set; }
        public int heightSize { get; set; }
        public string[,] board { get; set; }
        public int gCost = 0;

        //create two hashtables with openlist and closedlist
        public Dictionary<string, State> openList = new Dictionary<string,State>();
        public Dictionary<string, State> closedList = new Dictionary<string, State>();

        /// <summary>
        /// The A* Algorithm
        /// </summary>
        /// <param name="originalBoard">The start board</param>
        /// <param name="goal">The goal board</param>
        /// <returns>if true a path, if false no solution</returns>
        public bool AStarRun(string[,] originalBoard, string[,] goal)
        {
            //declare values
            int old_g_value = 0, parent_g = 0;
            string ChildHash = "";

            string[,] rootState = new string[heightSize, widthSize];
            string[,] theChildren = new string[heightSize, widthSize];
            string[,] theParent = new string[heightSize, widthSize];
            int ManhattenDistance = 0;
            int g_value = 0;
            //list of movables states
            List<string[,]> moveable = new List<string[,]>();
            //get start state
            rootState = copyOf(originalBoard);
            //add start state
            openList.Add(GetHash(rootState), new State(null, originalBoard, 0, getHeuristic(originalBoard)));
            //while openlist is not empty
            while (openList.Count != 0)
            {

                //Sort the openlist, so we are choosing the lowest f value
                var items = from pair in openList
                            orderby pair.Value.f_value ascending
                            select pair;

                //add the lowest possible f-value and save it to newState
                State newState = new State(items.First().Value.Child, items.First().Value.Parent, items.First().Value.g_value, items.First().Value.h_value);
                //get Parent state
                theParent = items.First().Value.Parent;
                //check if the parent has children
                if (newState.Child != null)
                {
                    //Child becomes a parent
                    theParent = copyOf(newState.Child);

                    //check goalstate
                    if (isEqual(newState.Child, goal))
                    {
                        openList.Remove(GetHash(theParent));
                        closedList.Add(GetHash(theParent), newState);
                        printMap(theParent);
                        //SOLUTION FOUND!
                        return true;
                    }

                }
                openList.Remove(GetHash(theParent));
                closedList.Add(GetHash(theParent), newState);

                moveable = moveableCheck(theParent);

                for (int i = 0; i < moveable.Count; i++)
                {
                    theChildren = copyOf(moveable[i]);
                    ChildHash = GetHash(theChildren);
                    ManhattenDistance = getHeuristic(theChildren);
                    g_value = newState.g_value + 1;
                    //if child is not in the closed list then continue
                    if (!closedList.ContainsKey(ChildHash))
                    {
                        //if child state is not in open list, then add it
                        if (!openList.ContainsKey(ChildHash))
                        {
                            openList.Add(ChildHash, new State(theChildren, theParent, g_value, ManhattenDistance));
                        }
                        else if (openList.ContainsKey(ChildHash))
                        {
                            //if its already in the list, this means that we have visited the state before. check if the path with the current state is better than the previous one
                            old_g_value = openList[ChildHash].g_value;       //get child g value aka old value
                            parent_g = g_value;                                 //get parent g value aka new value
                            if (parent_g < old_g_value)
                            {
                                //if true than update the key so its pointing to the new parent
                                openList.Remove(ChildHash);
                                openList.Add(ChildHash, new State(theChildren, theParent, g_value, ManhattenDistance));
                            }
                        }
                    }
                }
            }
            //if no path is found, return false
            return false;
        }

        /// <summary>
        /// Checks for movable positions for the robot
        /// </summary>
        /// <param name="origninalBoard">copy of the board</param>
        /// <returns>A list of movable states</returns>
        public List<string[,]> moveableCheck(string[,] origninalBoard)
        {



            //create string for board
            string[,] copyBoardLeft = new string[heightSize, widthSize];
            string[,] copyBoardRight = new string[heightSize, widthSize];
            string[,] copyBoardUp = new string[heightSize, widthSize];
            string[,] copyBoardDown = new string[heightSize, widthSize];

            List<string[,]> movableState = new List<string[,]>();
            List<Position> goalPosition = new List<Position>();
            //copy the originalboard to copies
            copyBoardLeft = copyOf(origninalBoard);
            copyBoardRight = copyOf(origninalBoard);
            copyBoardUp = copyOf(origninalBoard);
            copyBoardDown = copyOf(origninalBoard);

            goalPosition = getGoal(board);



            //::::::::::::::::::::::::::::::::::::::::CHECK LEFT::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            int i = getPosition(copyBoardLeft).xCoord;
            int j = getPosition(copyBoardLeft).yCoord;

            if (copyBoardLeft[i, j - 1] == ".")
            {
                copyBoardLeft[i, j - 1] = "M";
                copyBoardLeft[i, j] = ".";
                ////if we cross a goal, instead of deleting the goal, reinsert it
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }

                movableState.Add(deadlockvoid(copyBoardLeft));

            }

            else if (copyBoardLeft[i, j - 1] == "O")
            {
                copyBoardLeft[i, j - 1] = "M";
                copyBoardLeft[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }

                movableState.Add(deadlockvoid(copyBoardLeft));
            }

            else if (copyBoardLeft[i, j - 1] == "G")
            {
                copyBoardLeft[i, j - 1] = "M";
                copyBoardLeft[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardLeft));
            }

            else if (copyBoardLeft[i, j - 1] == "J")
            {
                //case of we moving a diamond. make sure we dont hit a wall or a deadlock
                if (copyBoardLeft[i, j - 2] != "J" && copyBoardLeft[i, j - 2] != "X" && copyBoardLeft[i, j - 2] != "O")
                {
                    copyBoardLeft[i, j - 1] = "M";
                    copyBoardLeft[i, j - 2] = "J";
                    copyBoardLeft[i, j] = ".";
                    //if we cross a goal, instead of deleting the goal, reinsert it
                    for (int q = 0; q < goalPosition.Count; q++)
                    {
                        if (copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                            copyBoardLeft[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                    }
                    movableState.Add(deadlockvoid(copyBoardLeft));

                }
            }

            //::::::::::::::::::::::::::::::::::::::::CHECK RIGHT::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            i = getPosition(copyBoardRight).xCoord;
            j = getPosition(copyBoardRight).yCoord;

            if (copyBoardRight[i, j + 1] == ".")
            {
                copyBoardRight[i, j + 1] = "M";
                copyBoardRight[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardRight));

            }

            else if (copyBoardRight[i, j + 1] == "O")
            {
                copyBoardRight[i, j + 1] = "M";
                copyBoardRight[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardRight));
            }

            else if (copyBoardRight[i, j + 1] == "G")
            {
                copyBoardRight[i, j + 1] = "M";
                copyBoardRight[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardRight));
            }

            else if (copyBoardRight[i, j + 1] == "J")
            {
                if (copyBoardRight[i, j + 2] != "J" && copyBoardRight[i, j + 2] != "X" && copyBoardRight[i, j + 2] != "O")
                {
                    copyBoardRight[i, j + 1] = "M";
                    copyBoardRight[i, j + 2] = "J";
                    copyBoardRight[i, j] = ".";
                    for (int q = 0; q < goalPosition.Count; q++)
                    {
                        if (copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                            copyBoardRight[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                    }

                    movableState.Add(deadlockvoid(copyBoardRight));

                }
            }

            //::::::::::::::::::::::::::::::::::::::::CHECK UP::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            i = getPosition(copyBoardUp).xCoord;
            j = getPosition(copyBoardUp).yCoord;

            if (copyBoardUp[i - 1, j] == ".")
            {
                copyBoardUp[i - 1, j] = "M";
                copyBoardUp[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardUp));

            }

            else if (copyBoardUp[i - 1, j] == "O")
            {
                copyBoardUp[i - 1, j] = "M";
                copyBoardUp[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardUp));

            }

            else if (copyBoardUp[i - 1, j] == "G")
            {
                copyBoardUp[i - 1, j] = "M";
                copyBoardUp[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardUp));
            }

            else if (copyBoardUp[i - 1, j] == "J")
            {
                if (copyBoardUp[i - 2, j] != "J" && copyBoardUp[i - 2, j] != "X" && copyBoardUp[i - 2, j] != "O")
                {
                    copyBoardUp[i - 1, j] = "M";
                    copyBoardUp[i - 2, j] = "J";
                    copyBoardUp[i, j] = ".";
                    for (int q = 0; q < goalPosition.Count; q++)
                    {
                        if (copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                            copyBoardUp[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                    }
                    movableState.Add(deadlockvoid(copyBoardUp));

                }
            }

            //::::::::::::::::::::::::::::::::::::::::CHECK DOWN::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            i = getPosition(copyBoardDown).xCoord;
            j = getPosition(copyBoardDown).yCoord;

            if (copyBoardDown[i + 1, j] == ".")
            {
                copyBoardDown[i + 1, j] = "M";
                copyBoardDown[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardDown));
            }

            else if (copyBoardDown[i + 1, j] == "O")
            {
                copyBoardDown[i + 1, j] = "M";
                copyBoardDown[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardDown));
            }

            else if (copyBoardDown[i + 1, j] == "G")
            {
                copyBoardDown[i + 1, j] = "M";
                copyBoardDown[i, j] = ".";
                for (int q = 0; q < goalPosition.Count; q++)
                {
                    if (copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                        copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                }
                movableState.Add(deadlockvoid(copyBoardDown));
            }

            else if (copyBoardDown[i + 1, j] == "J")
            {
                if (copyBoardDown[i + 2, j] != "J" && copyBoardDown[i + 2, j] != "X" && copyBoardDown[i + 2, j] != "O")
                {
                    copyBoardDown[i + 1, j] = "M";
                    copyBoardDown[i + 2, j] = "J";
                    copyBoardDown[i, j] = ".";
                    for (int q = 0; q < goalPosition.Count; q++)
                    {
                        if (copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] == ".")
                            copyBoardDown[goalPosition[q].xCoord, goalPosition[q].yCoord] = "G";
                    }
                    movableState.Add(deadlockvoid(copyBoardDown));
                }
            }
            return movableState;
        }

        /// <summary>
        /// Finds all the available diamonds
        /// </summary>
        /// <param name="copyBoard">copy of the board</param>
        /// <returns>Returns a list of available diamonds</returns>
        public List<Position> getDiamonds(string[,] copyBoard)
        {
            List<Position> diamonds = new List<Position>();
            string[,] goal = new string[heightSize, widthSize];
            goal = goalBoard(board);

            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (copyBoard[i, j] == "J" && goal[i, j] != "J")
                    {
                        diamonds.Add(new Position(i, j));
                    }
                }
            }
            return diamonds;
        }

        /// <summary>
        /// Finds all the available diamonds
        /// </summary>
        /// <param name="copyBoard">copy of the board</param>
        /// <returns>Returns a list of available goals</returns>
        public List<Position> getGoal(string[,] copyBoard)
        {
            List<Position> goals = new List<Position>();
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (copyBoard[i, j] == "G")
                    {
                        goals.Add(new Position(i, j));
                    }
                }
            }
            return goals;
        }

        /// <summary>
        /// Finds the position of the Robot (marked by M)
        /// </summary>
        /// <param name="copyBoard">copy of the board</param>
        /// <returns>Return the position of the robot</returns>
        public Position getPosition(string[,] copyBoard)
        {
            Position position = new Position();
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (copyBoard[i, j] == "M")
                    {
                        position.xCoord = i;
                        position.yCoord = j;
                        return position;
                    }
                }
            }
            return position;
        }

        /// <summary>
        /// board will be modified with all the diamonds in the goal position. The robot will be deleted from the map.
        /// </summary>
        /// <param name="startBoard">copy of the board</param>
        /// <returns>Returns a board with all the diamonds in the goal position</returns>
        public string[,] goalBoard(string[,] startBoard)
        {
            startBoard = deadlockvoid(startBoard);
            string[,] goal = new string[heightSize, widthSize];
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (startBoard[i, j] == "O")
                        goal[i, j] = "O";
                    if (startBoard[i, j] == ".")
                        goal[i, j] = ".";
                    if (startBoard[i, j] == "X")
                        goal[i, j] = "X";
                    if (startBoard[i, j] == "J")
                        goal[i, j] = ".";
                    if (startBoard[i, j] == "M")
                        goal[i, j] = ".";

                }
            }
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (startBoard[i, j] == "G")
                        goal[i, j] = "J";

                }
            }
            return goal;
        }

        /// <summary>
        /// finds each distance from diamond to goal and returns the total value and taking the robot position into account
        /// </summary>
        /// <param name="copyBoard">copy of the board</param>
        /// <returns>Returns H-Value</returns>
        public int getHeuristic3(string[,] copyBoard)
        {
            int ManhattenDistance = 0;
            int tox = 0, toy = 0, fromx = 0, fromy = 0;
            int x = getPosition(copyBoard).xCoord;
            int y = getPosition(copyBoard).yCoord;
            List<Position> GoalPosition = new List<Position>();
            List<Position> DiamondPosition = new List<Position>();
            GoalPosition = getGoal(copyBoard);
            DiamondPosition = getDiamonds(copyBoard);
            for (int i = 0; i < GoalPosition.Count; i++)
            {
                fromx = DiamondPosition[i].xCoord;
                fromy = DiamondPosition[i].yCoord;
                ManhattenDistance += Math.Abs(fromx - x) + Math.Abs(fromy - y);

                tox = GoalPosition[i].xCoord;
                toy = GoalPosition[i].yCoord;
                
                ManhattenDistance += Math.Abs(fromx - tox) + Math.Abs(fromy - toy);
            }
            return ManhattenDistance;
        }

        /// <summary>
        /// finds each distance from diamond to goal and returns the total value. the diamonds is found by search from upper left position
        /// </summary>
        /// <param name="copyBoard">copy of the board</param>
        /// <returns>Returns H-Value</returns>
        public int getHeuristic2(string[,] copyBoard)
        {
            
            int ManhattenDistance = 0;
            int tox = 0, toy = 0, fromx = 0, fromy = 0;
            List<Position> GoalPosition = new List<Position>();
            List<Position> DiamondPosition = new List<Position>();
            GoalPosition = getGoal(copyBoard);
            DiamondPosition = getDiamonds(copyBoard);
            
            for (int i = 0; i < GoalPosition.Count; i++)
            {
                tox = GoalPosition[i].xCoord;
                toy = GoalPosition[i].yCoord;
                fromx = DiamondPosition[i].xCoord;
                fromy = DiamondPosition[i].yCoord;
                ManhattenDistance += Math.Abs(fromx - tox) + Math.Abs(fromy - toy);
            }
            return ManhattenDistance;
        }

        /// <summary>
        /// finds each minimum distance from diamond to goal and returns the total value
        /// </summary>
        /// <param name="copyBoard">copy of the board</param>
        /// <returns>Returns H-Value</returns>
        public int getHeuristic(string[,] copyBoard)
        {
            
            int ManhattenDistance = 0, oldMD = 1000000;
            int tox = 0, toy = 0, fromx = 0 , fromy = 0;
            List<Position> GoalPosition = new List<Position>();
            List<Position> DiamondPosition = new List<Position>();
            int[,] min = new int[widthSize,heightSize];
            GoalPosition = getGoal(copyBoard);
            DiamondPosition = getDiamonds(copyBoard);
            int minIndex = 100;
            //finds each minimum distance from diamond to goal and returns the total value
            for (int i = 0; i < GoalPosition.Count; i++)
            {
                tox = GoalPosition[i].xCoord;
                toy = GoalPosition[i].yCoord;
                oldMD = 1000000; //restart
                minIndex = 100; //restart
                for (int j = 0; j < DiamondPosition.Count; j++)
                {
                    fromx = DiamondPosition[j].xCoord;
                    fromy = DiamondPosition[j].yCoord;
                    int minimumMD = Math.Abs(fromx - tox) + Math.Abs(fromy - toy);
                    if (minimumMD < oldMD)
                    {
                        oldMD = minimumMD;
                        //if we know the minimum value, we have to save the index, so we can remove the diamond from the list. This way, we can ensure we dont encounter the same diamond again next time
                        minIndex = j;
                    }
                }
                ManhattenDistance += oldMD;
                //check if we get a right index number. If it is not the right index number the default value is 100 and will skip the remove function
                if(minIndex != 100)
                    DiamondPosition.RemoveAt(minIndex);


            }
            return ManhattenDistance;
        }

        /// <summary>
        /// Compares two states. 
        /// </summary>
        /// <param name="start">the first state</param>
        /// <param name="goal">the second state</param>
        /// <returns>if equal returns true, if not returns false</returns>
        public bool isEqual(string[,] start, string[,] goal)
        {
            string[,] startcopy = new string[heightSize, widthSize];
            startcopy = copyOf(start);
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (start[i, j] == "M")
                        startcopy[i, j] = ".";
                    if (goal[i, j] == "M")
                        goal[i, j] = ".";
                }
            }
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    
                    if (startcopy[i, j] != goal[i, j])
                        return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Detects deadlocks and marks the deadlock position with O.
        /// </summary>
        /// <param name="oboard">copy of the board</param>
        /// <returns>Returns the board with deadlock marked by "O"</returns>
        public string[,] deadlockvoid(string[,] oboard)
        {
            string[,] copyBoard = new string[heightSize, widthSize];
            for (int i = 1; i < heightSize-1; i++)
            {
                for (int j = 1; j < widthSize-1; j++)
                {
                    //check first corner up left
                    if (oboard[i, j - 1] == "X" && oboard[i - 1, j - 1] == "X" && oboard[i - 1, j] == "X" && oboard[i, j] == ".")
                    {
                        if (oboard[i, j] != "J" || oboard[i, j] != "G" || oboard[i, j] != "M")
                            oboard[i, j] = "O";
                    }
                    //check first corner up right
                    if (oboard[i - 1, j] == "X" && oboard[i - 1, j + 1] == "X" && oboard[i, j + 1] == "X" && oboard[i, j] == ".")
                    {
                        if (oboard[i, j] != "J" || oboard[i, j] != "G" || oboard[i, j] != "M")
                            oboard[i, j] = "O";
                    }
                    //check first corner down right
                    if (oboard[i, j + 1] == "X" && oboard[i + 1, j + 1] == "X" && oboard[i + 1, j] == "X" && oboard[i, j] == ".")
                    {
                        if (oboard[i, j] != "J" || oboard[i, j] != "G" || oboard[i, j] != "M")
                            oboard[i, j] = "O";
                    }
                    //check first corner down left
                    if (oboard[i + 1, j] == "X" && oboard[i + 1, j-1] == "X" && oboard[i, j - 1] == "X" && oboard[i, j] == ".")
                    {
                        if(oboard[i,j] != "J" || oboard[i,j] != "G" || oboard[i,j] != "M")
                            oboard[i, j] = "O";
                    }
                }
            }
            return oboard;
        }

        /// <summary>
        /// Makes a Hash string of the board
        /// </summary>
        /// <param name="mapCopy">copy of the board</param>
        /// <returns>The Hash string</returns>
        public string GetHash(string[,] mapCopy)
        {
            string HashResult = "";
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (mapCopy[i, j] == "X")
                    {
                        HashResult += "X";
                    }
                    if (mapCopy[i, j] == " ")
                    {
                        HashResult += " ";
                    }
                    if (mapCopy[i, j] == ".")
                    {
                        HashResult += ".";
                    }
                    if (mapCopy[i, j] == "G")
                    {
                        HashResult += "G";
                    }
                    if (mapCopy[i, j] == "J")
                    {
                        HashResult += "J";
                    }
                    if (mapCopy[i, j] == "M")
                    {
                        HashResult += "M";
                    }
                    if (mapCopy[i, j] == "O")
                    {
                        HashResult += "O";
                    }
                }
            }
            return HashResult;
        }

        /// <summary>
        /// Makes a copy of the input board
        /// </summary>
        /// <param name="oriBoard">input board</param>
        /// <returns>copy of the board</returns>
        public string[,] copyOf(string[,] oriBoard)
        {
            string[,] copyBoard = new string[heightSize, widthSize];

            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    copyBoard[i, j] = oriBoard[i, j];
                }
            }
            return copyBoard;
        }

        /// <summary>
        /// Prints the Board
        /// </summary>
        /// <param name="mapPrint">Board to to printed to console</param>
        public void printMap(string[,] mapPrint)
        {
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    Console.Write(mapPrint[i, j] + " ");
                }
                Console.Write("\n");
            }
        }

        /// <summary>
        /// When recontructing the path, the difference between the parent and child state can tell the direction
        /// </summary>
        /// <param name="childState">The Child State</param>
        /// <param name="parentState">The Parent State</param>
        /// <returns>The direction</returns>
        public string getDirection(string[,] childState, string[,] parentState)
        {
            string path = "";
            for (int i = 0; i < heightSize; i++)
            {
                for (int j = 0; j < widthSize; j++)
                {
                    if (parentState[i, j] == "M")
                    {
                        if (childState[i - 1, j] == "M" && childState[i - 2, j] == "J")
                            path = "U";
                        else if (childState[i - 1, j] == "M")
                            path = "u";

                        if (childState[i + 1, j] == "M" && childState[i + 2, j] == "J")
                            path = "D";
                        else if (childState[i + 1, j] == "M")
                            path = "d";

                        if (childState[i, j - 1] == "M" && childState[i, j - 2] == "J")
                            path = "L";
                        else if (childState[i, j - 1] == "M")
                            path = "l";

                        if (childState[i, j + 1] == "M" && childState[i, j + 2] == "J")
                            path = "R";
                        else if (childState[i, j + 1] == "M")
                            path = "r";
                    }
                }
            }
            return path;
        }

        /// <summary>
        /// Recontruct path when the goal is found
        /// </summary>
        /// <returns>the goal path</returns>
        public string recontructPath()
        {
            string path = "";
            char[] pathReverse;
            string[,] child = new string[heightSize, widthSize];
            string[,] parent = new string[heightSize, widthSize];
            child = closedList.Last().Value.Child;
            if (closedList[GetHash(child)].Child != null)
            {
                while (closedList[GetHash(child)].Child != null)
                {
                    parent = copyOf(closedList[GetHash(child)].Parent);
                    //get direction
                    path += getDirection(child, parent);
                    //set the parent to child
                    child = copyOf(parent);
                }
            }
            pathReverse = path.ToCharArray();
            Array.Reverse(pathReverse);
            return new string(pathReverse);

        }

    }
}
