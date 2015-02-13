using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace SokobanSolver
{
    class Agent
    {
        
        int getwidth;
        int getheight;
        public string[,] loadMap(string path)
        {
            string[,] map;   
            //save map to lines where each lines is saves to a array index
            //@"C:\\Users\\Anochjhn Iruthayam\\Documents\\Visual Studio 2012\\Projects\\SearchAlg\\SearchAlg\\map-file.txt"
            string[] lines = System.IO.File.ReadAllLines(path);
            
            //get first line to get information about size and diamonds
            string tmp = lines[0];
            getwidth = int.Parse(tmp[0] + "" + tmp[1]);
            getheight = int.Parse(tmp[3] + "" + tmp[4]);
            int numDiamond = int.Parse(tmp[6] + "" + tmp[7]);
            //print out information to console
            Console.WriteLine("Width Size : " + getwidth + "\n");
            Console.WriteLine("Height Size : " + getheight + "\n");
            Console.WriteLine("Number of Diamonds : " + numDiamond + "\n");

            //create map
            map = new string[getheight, getwidth];
            //load date to map by reading one line at the time
            //set i = 1 to ignore the information about the map
            for (int i = 1; i < getheight+1; i++)
            {
                tmp = lines[i];
                for (int j = 0; j < getwidth; j++)
                {
                    map[i - 1, j] = lines[i][j].ToString();
                }
            }
            return map;
        }
        public void runGame()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //long start = DateTime.Now.Second;
            string[,] map = loadMap("map_for_competition.txt");
            string path = "";
            GameBoard board = new GameBoard(map, getwidth, getheight);

            board.printMap(board.deadlockvoid(board.board));
            string[,] goalState = new string[board.heightSize, board.widthSize];
            goalState = board.goalBoard(board.board);

            Console.WriteLine();
            board.printMap(goalState);


            Console.WriteLine("A* in process!");
            if (board.AStarRun(board.board, goalState))
            {
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);
                Console.WriteLine("Path found!");
                path = board.recontructPath();
                Console.WriteLine(path + "\nPath Length: " + path.Count() + "\nTotal time: " + stopwatch.Elapsed + "\nHeuristic: diamond to goal shortest path");
                //System.IO.File.WriteAllText(GameMapPath + "path.txt", path);
                System.IO.File.WriteAllText("pathThreeDiamondswithMposition2.txt", 
                    "Path:" + path + 
                    "\nPath Length: " + path.Count() + 
                    "\nTotal time: " + stopwatch.ElapsedMilliseconds + 
                    "\nHeuristic: position man");
            }
            else
                Console.WriteLine("Path not found!");
        }
    }
}
