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
           // OpenMazeAndGenerateFile("inputSmall.png");
            GenerateMazeAndSaveIt(100, 100);
        }


        public static void OpenMazeAndGenerateFile(string fileName)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(fileName);



            var maze = new Maze(bmp.Width, bmp.Height);
            maze.LoadFileAsMap(fileName);

            var path = PathFinder.Find(new MPoint(1, 1), new MPoint(bmp.Width - 3, bmp.Height - 3), maze.Map);


            maze.Save("MustBeSameAsInput.png", ImageFormat.Png);
            maze.SolveAndSave("SolvedMaze.Png", ImageFormat.Png, path);
        }


        
        /// <summary>
        /// Generates a map then saves the g
        /// </summary>
        /// <param name="width">The width for the map</param>
        /// <param name="height">The height for the map</param>
        public static void GenerateMazeAndSaveIt(int width, int height)
        {
            var o = new MazeGenerator();
            var maze = o.Generate(width, height);

            var startingPoint = new MPoint(1, 1); 
            var endPoint = new MPoint(width - 3, height - 3);

            var path = PathFinder.Find(startingPoint, endPoint, maze.Map);
            maze.Save("generatedMaze.png", ImageFormat.Png);

            maze.SolveAndSave("generatedMazeSolved1.png", ImageFormat.Png, path);
        }



    }
}
