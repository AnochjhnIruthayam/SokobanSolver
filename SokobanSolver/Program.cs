using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //The Map should be in the Debug or Release folder. If solution exist the path will be saved there as well
            Agent Smith = new Agent();
            Smith.runGame();
            System.Console.ReadKey();
        }
    }
}
