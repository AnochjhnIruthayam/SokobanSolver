using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanSolver
{
    class Position
    {
        public int xCoord { get; set; }
        public int yCoord { get; set; }

        public Position(int _x, int _y)
        {
            xCoord = _x;
            yCoord = _y;
        }
        public Position() { }
    }
}
