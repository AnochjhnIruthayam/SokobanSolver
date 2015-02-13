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
            Agent Smith = new Agent();
            Smith.runGame("C:\\Users\\Anochjhn Iruthayam\\Documents\\Visual Studio 2012\\Projects\\SearchAlg\\SearchAlg\\");
            System.Console.ReadKey();
        }
    }
}
