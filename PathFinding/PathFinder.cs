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
        
        /// <summary>
        /// This is a Depth-First search implementation where the frontier acts like a last-in first-out stack. 
        /// The elements are added to the stack one at a time. 
        /// The one selected and taken off the frontier at any time is the last element that was added.
        /// </summary>
        public static List<MPoint> DepthFirstSearch(MPoint start, MPoint end, Map map)
        {
            int width = map.Width;
            int height = map.Height;

            Map visitedPoints = new Map(width, height);

            List<MPoint> points = new List<MPoint>();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width || y == height)
                    {
                        visitedPoints[x, y] = true;
                    }
                }
            }

            Stack<MPoint> stack = new Stack<MPoint>();
            stack.Push(start);
            visitedPoints[start.X, start.Y] = true;

            while (stack.Count != 0)
            {
                MPoint cur = stack.Peek();
                int x = cur.X;
                int y = cur.Y;

                if (end.X == x && end.Y == y)
                {
                    break;
                }

                MPoint target = new MPoint(-1, -1);
                if (IsValid(x + 1, y, map, visitedPoints, width, height))
                {
                    target = new MPoint(x + 1, y);
                }
                else if (IsValid(x, y + 1, map, visitedPoints, width, height))
                {
                    target = new MPoint(x, y + 1);
                }
                else if (IsValid(x - 1, y, map, visitedPoints, width, height))
                {
                    target = new MPoint(x - 1, y);
                }
                else if (IsValid(x, y - 1, map, visitedPoints, width, height))
                {
                    target = new MPoint(x, y - 1);
                }
                if (target.X != -1)
                {
                    stack.Push(target);
                    visitedPoints[target.X, target.Y] = true;
                }
                else
                {
                    stack.Pop();
                }
            }

            points.AddRange(stack);
            points.Reverse();
            return points;
        }

        /// <summary>
        /// A helper method that will allow me to compare if a point is valid or not 
        /// by verifying if point existed in the visitedMap
        /// </summary>
        /// <returns>true if point is valid, false otherwise</returns>
        private static Boolean IsValid(int x, int y, Map map, Map visitedMap, int width, int height)
        {
            if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
            {
                if (visitedMap[x, y])
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
    }
}
