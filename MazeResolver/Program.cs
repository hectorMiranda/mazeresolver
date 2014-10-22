using PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace MazeResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenMazeAndGenerateFile("input.png");
            GenerateMazeAndSolveIt(50, 50);
        }

        public static void OpenMazeAndGenerateFile(string fileName)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(fileName);
            var maze = new Maze(bmp);

            maze.Save("MustBeSameAsInput.png", ImageFormat.Png);

            var path = PathFinder.FindDepthFirst(new MPoint(1, 1), new MPoint(bmp.Width - 13, bmp.Height - 13), maze.Map);
            
            maze.Save("SolvedMaze.Png", ImageFormat.Png, path);
        }


        
        /// <summary>
        /// Generates, solves and save a maze
        /// </summary>
        /// <param name="width">The width for the map</param>
        /// <param name="height">The height for the map</param>
        public static void GenerateMazeAndSolveIt(int width, int height)
        {
            var o = new MazeGenerator();
            var maze = o.Generate(width, height);

            var startingPoint = new MPoint(1, 1); 
            var endPoint = new MPoint(width - 3, height - 3);

            var path = PathFinder.FindDepthFirst(startingPoint, endPoint, maze.Map);
            maze.Save("generatedMaze.png", ImageFormat.Png);

            maze.Save("generatedMazeSolved1.png", ImageFormat.Png, path);
        }



    }
}
