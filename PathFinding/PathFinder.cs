using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class PathFinder
    {
        private static Random r = new Random();
        public static List<MPoint> FindDepthFirst(MPoint start, MPoint end, Map map)
        {
            int width = map.Width;
            int height = map.Height;

            Map visitedMap = new Map(width, height);
            Map visited = new Map(width, height);

            List<MPoint> pointlist = new List<MPoint>();

            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width || y == height)
                    {
                        visited[x, y] = true;
                    }
                }
            }


            //Hier begint het gedoe
            Stack<MPoint> stackje = new Stack<MPoint>();
            stackje.Push(start);
            visited[start.X, start.Y] = true;

            while (stackje.Count != 0)
            {
                MPoint cur = stackje.Peek();
                int x = cur.X;
                int y = cur.Y;

                if (end.X == x && end.Y == y)
                {
                    break;
                }

                MPoint target = new MPoint(-1, -1);
                if (IsValid(x + 1, y, map, visited, width, height))
                {
                    target = new MPoint(x + 1, y);
                }
                else if (IsValid(x, y + 1, map, visited, width, height))
                {
                    target = new MPoint(x, y + 1);
                }
                else if (IsValid(x - 1, y, map, visited, width, height))
                {
                    target = new MPoint(x - 1, y);
                }
                else if (IsValid(x, y - 1, map, visited, width, height))
                {
                    target = new MPoint(x, y - 1);
                }

                if (target.X != -1)
                {
                    stackje.Push(target);
                    visited[target.X, target.Y] = true;
                }
                else
                {
                    stackje.Pop();
                }
            }

            pointlist.AddRange(stackje);

            pointlist.Reverse();

            return pointlist;
        
        }


        private static Boolean IsValid(int x, int y, Map map, Map visitedmap, int width, int height)
        {
            if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
            {
                if (visitedmap[x, y])
                {
                    return false;
                }
                else
                {
                    return map[x, y];
                }
            }
            else
            {
                return false;
            }
        }

        private static Boolean isValid(int x, int y, Map map, int width, int height)
        {
            if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
            {
                return map[x, y];
            }
            else
            {
                return false;
            }
        }





    }
}
