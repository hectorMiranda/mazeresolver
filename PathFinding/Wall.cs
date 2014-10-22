using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class Wall
    {
        public int xStart;
        public int yStart;
        public int xEnd;
        public int yEnd;

        public Wall(int xStart, int yStart, int xEnd, int yEnd)
        {
            this.xStart = xStart;
            this.yStart = yStart;
            this.xEnd = xEnd;
            this.yEnd = yEnd;
        }
    }

    

}
