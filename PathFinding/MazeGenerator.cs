using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    /// <summary>
    /// Helper class that allow us to test the pathfinder with multiple and random inputs
    /// without having to have a file for testing.
    /// </summary>
    public class MazeGenerator 
    {
        /// <summary>
        /// Generates a Maze
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        /// <returns>A maze object with an internal map</returns>
        public Maze Generate(int width, int height)
        {
            Maze maze = new Maze(width, height);
            GenerateMap(maze, new Random());
            return maze;

        }

        /// <summary>
        /// Generates a Maze, allowing us to provide a seed
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        /// <param name="seed">Seed used to generate the maze</param>
        /// <returns>A maze object with an internal map</returns>
        public Maze Generate(int width, int height, int seed)
        {
            Maze maze = new Maze(width, height);
            GenerateMap(maze, new Random(seed));
            return maze;
        }


        /// <summary>
        /// Generates the map for the maze by validating each of the target points.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="maze"></param>
        /// <param name="r"></param>
        private void GenerateMap(Maze maze, Random r)
        {
            long steps = ((maze.Width - 1) / 2) * ((maze.Height - 1) / 2);
            long currentStep = 1;

            int x = 1;
            int y = 1;

            var stack = new Stack<MPoint>();

            stack.Push(new MPoint(x, y));

            maze.map[x, y] = true;

            MPoint[] targets = new MPoint[4];

            while (stack.Count != 0)
            {
                MPoint currentPoint = stack.Peek();
                int targetCounter = 0;

                x = currentPoint.X;
                y = currentPoint.Y;

                if (IsValid(x - 2, y, maze.map, maze))
                {
                    targets[targetCounter].X = x - 2;
                    targets[targetCounter].Y = y;
                    targetCounter++;
                }
                if (IsValid(x + 2, y, maze.map, maze))
                {
                    targets[targetCounter].X = x + 2;
                    targets[targetCounter].Y = y;
                    targetCounter++;
                }
                if (IsValid(x, y - 2, maze.map, maze))
                {
                    targets[targetCounter].X = x;
                    targets[targetCounter].Y = y - 2;
                    targetCounter++;
                }
                if (IsValid(x, y + 2, maze.map, maze))
                {
                    targets[targetCounter].X = x;
                    targets[targetCounter].Y = y + 2;
                    targetCounter++;
                }

                if (targetCounter > 0)
                {
                    var target = targets[r.Next(targetCounter)];
                    stack.Push(target);
                    maze.map[target.X, target.Y] = true;

                    currentStep++;

                    if (target.X < x)
                    {
                        maze.map[x - 1, y] = true;
    
                    }
                    else if (target.X > x)
                    {
                        maze.map[x + 1, y] = true;
                       
                    }
                    else if (target.Y < y)
                    {
                        maze.map[x, y - 1] = true;
                       
                    }
                    else if (target.Y > y)
                    {
                        maze.map[x, y + 1] = true;
                       
                    }
                }
                else
                {
                    stack.Pop();
                }


            }
        }

        private static bool IsValid(int x, int y, Map map, Maze maze)
        {
            if (x > 0 && x < maze.Width - 1 && y > 0 && y < maze.Height - 1)
            {
                return !map[x, y];
            }
            return false;
        }
    }
}
