using PathFinding;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace MazeResolver
{
    /// <summary>
    /// Testing console for the Bluebeam code project.
    /// </summary>
    /// <remarks>
    /// My original intention was to write a Powershell module, which would allowed me to keep the code in a self-contained resuable unit,
    /// however time ran out fast and I just choose to stick to the idea of the console as this will keep things simple.
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Count()>=1)
                ProcessFile(args);
            else
                EnterTestingMode();
        }

        
        /// <summary>
        /// Extracts parameter info and process maze, writing output image solution to disk
        /// </summary>
        /// <remarks>With more time I would had added log4net for logging and exception handling purposes.</remarks>
        private static void ProcessFile(string[] args)
        {
            string input = args[0];
            string output = string.Format("{0}_output{1}", Path.GetFileNameWithoutExtension(input), Path.GetExtension(input)); 
            ImageFormat outputFormat = ImageFormat.Png; 

            if (args.Count() > 1)
            {
                output = args[1];

                switch (Path.GetExtension(output.ToLower()))
                { 
                    case "png":
                        outputFormat = ImageFormat.Png;
                        break;
                    case "bmp":
                        outputFormat = ImageFormat.Bmp;
                        break;
                    case "jpg":
                        outputFormat = ImageFormat.Png;
                        break;
                }
            }

            try
            {
                Console.WriteLine(string.Format("Processing {0} ...", input));

                var maze = new Maze((Bitmap)Image.FromFile(input));
                var startTime = DateTime.UtcNow;
                var path = PathFinder.DepthFirstSearch(maze.Start, maze.End, maze.Map);
                var totalTime = (DateTime.UtcNow - startTime).Milliseconds;
                Console.WriteLine(string.Format("Finding path took: {0} ms", totalTime));
                maze.Save(output, outputFormat, path);
                
                Console.WriteLine(string.Format("File {0} has being created ...", output));
            }
            catch (Exception up)
            {
                if (up is FileNotFoundException || up is OutOfMemoryException)
                    Console.WriteLine(string.Format("Exception: {0}", up.Message));                 
                else
                    throw up; // :)
            }

        }



        /// <summary>
        /// Due to time constrains I decided to test this project by generating a new map A in memory, writing A to disk, finding the path and finally writing a new map B to disk.
        /// In a proper production environment a test project would be added containing all the test methods there. 
        /// </summary>
        private static void EnterTestingMode()
        {
            Console.WriteLine("Maze Resolver 0.1 ");
            Console.WriteLine("You are entering testing mode, to avoid this behavior use the following syntax:");
            Console.WriteLine("maze.exe inputImageFile [outFile]");

            int width;
            int height;

            Console.WriteLine("Enter width:");
            if (!int.TryParse(Console.ReadLine(), out width))
                width = 300;
            Console.WriteLine("Enter height:");
            if (!int.TryParse(Console.ReadLine(), out height))
                height = 300;

            string mazeFileName = "TestMaze.png";
            string mazeFileNameSolved = "TestMaze_solved.png";

            var o = new MazeGenerator();
            Console.WriteLine("Generating Maze");
            var maze = o.Generate(width, height);

            var startingPoint = new MPoint(1, 1);
            var endPoint = new MPoint(width - 3, height - 3);

            Console.WriteLine("Finding path");
            var startTime = DateTime.UtcNow;
            var path = PathFinder.DepthFirstSearch(startingPoint, endPoint, maze.Map);
            var totalTime = (DateTime.UtcNow - startTime).Milliseconds;
            Console.WriteLine(string.Format("Finding path took: {0} ms",totalTime ));


            Console.WriteLine(string.Format("Generated files are {0} : {1}", mazeFileName, mazeFileNameSolved));
            maze.Save(mazeFileName, ImageFormat.Png); 
            maze.Save(mazeFileNameSolved, ImageFormat.Png, path);
        }
    }
}
