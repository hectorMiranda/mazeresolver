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
        /// Finds the path between the start and the endpoint in a maze
        /// </summary>
        /// <param name="start">The start point</param>
        /// <param name="end">The end point</param>
        /// <param name="map">The maze.InnerMap</param>
        /// <param name="callBack">The callback that can be used to see what the pathfinder is doing (or null), the boolean true = a new path find thingy or false when it determined that path is not correct</param>
        /// <returns>The shortest path in a list of points</returns>
        public static List<MPoint> Find(MPoint start, MPoint end, Map map)
        {
            int width = map.Width;
            int height = map.Height;

            List<MPoint> stackje = new List<MPoint>();
            stackje.Add(start);

            MPoint cur = new MPoint();
            MPoint prev = new MPoint(-1, -1);

            var lastBackTrackDir = -1;

            while (stackje.Count != 0)
            {
                cur = stackje[stackje.Count - 1];
                var x = cur.X;
                var y = cur.Y;

                MPoint target = new MPoint(-1, -1);
                //Make sure the point was not the previous point, also make sure that the point is white, also make sure that if we backtracked we don't go to a direction we already went to
                if ((prev.X != x + 1 || prev.Y != y) && isValid(x + 1, y, map, width, height) && lastBackTrackDir < 0)
                {
                    target = new MPoint(x + 1, y);
                }
                else if ((prev.X != x || prev.Y != y + 1) && isValid(x, y + 1, map, width, height) && lastBackTrackDir < 1)
                {
                    target = new MPoint(x, y + 1);
                }
                else if ((prev.X != x - 1 || prev.Y != y) && isValid(x - 1, y, map, width, height) && lastBackTrackDir < 2)
                {
                    target = new MPoint(x - 1, y);
                }
                else if ((prev.X != x || prev.Y != y - 1) && isValid(x, y - 1, map, width, height) && lastBackTrackDir < 3)
                {
                    target = new MPoint(x, y - 1);
                }
                else
                {
                    var prepoppy = stackje[stackje.Count - 1];
                    stackje.RemoveAt(stackje.Count - 1);

                    if (stackje.Count == 0)
                    {
                        //No path found
                        break;
                    }

                    var newcur = stackje[stackje.Count - 1];

                    //Set the new previous point
                    if (stackje.Count == 1)
                    {
                        prev = new MPoint(-1, -1);
                    }
                    else
                    {
                        prev = stackje.ElementAt(stackje.Count - 2);
                    }

                    //Console.WriteLine("Backtracking to X: " + newcur.X + " Y: " + newcur.Y);
                    //Console.WriteLine("Setting new prev: " + prev.X + " Y: " + prev.Y);

                    //Set the direction we backtracked from
                    if (prepoppy.X > newcur.X)
                    {
                        lastBackTrackDir = 0;
                    }
                    else if (prepoppy.Y > newcur.Y)
                    {
                        lastBackTrackDir = 1;
                    }
                    else if (prepoppy.X < newcur.X)
                    {
                        lastBackTrackDir = 2;
                    }
                    else if (prepoppy.Y < newcur.Y)
                    {
                        lastBackTrackDir = 3;
                    }

                    //Console.WriteLine("Lastbacktrackdir: " + lastBackTrackDir);
                    continue;

                }

                lastBackTrackDir = -1;

                //Console.WriteLine("Going to X: " + target.X + " Y: " + target.Y);

                stackje.Add(target);

                if (target.X == end.X && target.Y == end.Y)
                {
                    //Path found
                    break;
                }

                prev = cur;

            }

            return stackje;
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
