using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanSolver
{
    class State
    {
        public string[,] Child { get; set; }
        public string[,] Parent { get; set; }
        public int g_value { get; set; }
        public int h_value { get; set; }
        public int f_value 
        { 
            get 
            { 
                return g_value + h_value; 
            } 
        }

        public State(string[,] _Child, string[,] _Parent, int _g_value, int _h_value)
        {
            Child = _Child;
            Parent = _Parent;
            g_value = _g_value;
            h_value = _h_value;
        }

    }
}
