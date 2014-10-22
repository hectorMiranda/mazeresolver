using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class Maze
    {
        internal Map map;
        public void LoadFileAsMap(string fileName)
        {
            var bmp = (Bitmap)Image.FromFile(fileName);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var currentPixel = bmp.GetPixel(x, y);

                    if (currentPixel.ToArgb() == Color.Black.ToArgb())
                    {
                        map[x, y] = false;
                    }
                    else
                    {
                        map[x, y] = true;
                    }
                }
            }

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var firstPixel = bmp.GetPixel(x, y);

                    if (firstPixel.ToArgb() == Color.Black.ToArgb())
                    {
                        map[x, y] = false;
                    }
                    else
                    {
                        map[x, y] = true;
                    }

                }

            }
        }
        private void AddWall(List<Wall> walls, int xStart, int yStart, int xEnd, int yEnd)
        {
            if (xEnd - xStart <= 1 && yEnd - yStart <= 1)
            {
                return;
            }

            Wall wall = new Wall(xStart, yStart, xEnd, yEnd);
            walls.Add(wall);
        }
        public Map Map
        {
            get { return map; }
        }
        public Maze(int width, int height)
        {
            map = new InnerMap(width, height);
        }
        public Maze(Map concreteMap)
        {
            map = concreteMap;
        }
        public int Width
        {

            get { return map.Width; }
        }
        public int Height
        {
            get { return map.Height; }
        }
        public void Save(String fileName, ImageFormat imageFormat)
        {
            using (Bitmap bitmap = new Bitmap(map.Width - 1, map.Height - 1, PixelFormat.Format1bppIndexed))
            {

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                var bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
                var rgbValues = new byte[bytes];

                Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int y = 0; y < map.Height - 1; y++)
                {
                    int counter = bmpData.Stride * y;
                    int x = 0;
                    for (int i = 0; i < bmpData.Stride; i++)
                    {
                        if (x > bitmap.Width)
                        {
                            break;
                        }

                        BitArray bitArray = new BitArray(8);

                        for (int j = 7; j >= 0; j--)
                        {
                            if (x < map.Width)
                            {
                                bitArray[j] = map[x, y];
                                x++;
                            }
                        }
                        rgbValues[counter] = (byte)GetIntFromBitArray(bitArray);
                        counter++;
                    }
                }

                Marshal.Copy(rgbValues, 0, ptr, bytes);
                bitmap.UnlockBits(bmpData);
                bitmap.Save(fileName, imageFormat);
            }
        }
        private int GetIntFromBitArray(BitArray bitArray)
        {
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        private BitArray GetBitArrayFromByte(byte byteje)
        {
            BitArray b = new BitArray(new byte[1] { byteje });
            return b;
        }

        public void SolveAndSave(String fileName, ImageFormat imageFormat, List<MPoint> path)
        {

            using (Bitmap objBmpImage = new Bitmap(map.Width - 1, map.Height - 1, PixelFormat.Format4bppIndexed))
            {
                Rectangle rect = new Rectangle(0, 0, objBmpImage.Width, objBmpImage.Height);
                BitmapData bmpData = objBmpImage.LockBits(rect, ImageLockMode.ReadWrite, objBmpImage.PixelFormat);
                IntPtr pointer = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * objBmpImage.Height;
                byte[] rgbValues = new byte[bytes];

                Marshal.Copy(pointer, rgbValues, 0, bytes);

                for (int y = 0; y < map.Height - 1; y++)
                {
                    int counter = bmpData.Stride * y;
                    int x = 0;
                    for (int i = 0; i < bmpData.Stride; i++)
                    {
                        if (x > objBmpImage.Width)
                        {
                            break;
                        }

                        BitArray bitar = new BitArray(8);

                        for (int j = 4; j >= 0; j = j - 4)
                        {
                            if (map[x, y])
                            {
                                bitar[j + 3] = true;
                                bitar[j + 2] = true;
                                bitar[j + 1] = true;
                                bitar[j + 0] = true;
                            }
                            else
                            {
                                bitar[j + 3] = false;
                                bitar[j + 2] = false;
                                bitar[j + 1] = false;
                                bitar[j + 0] = false;
                            }
                            x++;
                        }
                        rgbValues[counter] = (byte)GetIntFromBitArray(bitar);
                        counter++;
                    }
                }

                for (int i = 0; i < path.Count; i++)
                {
                    long percent = 100L * (long)(i + 1) / (long)path.Count;
                    MPoint point = path[i];
                    int xrest = point.X % 2;

                    int pos = bmpData.Stride * point.Y + point.X / 2;

                    BitArray bitar = GetBitArrayFromByte(rgbValues[pos]);

                    int xtra = 4;

                    if (xrest >= 1)
                    {
                        xtra = 0;
                    }

                    //start represented as red
                    if (i == 0)
                    {
                        bitar[xtra + 3] = true;
                        bitar[xtra + 2] = false;
                        bitar[xtra + 1] = false;
                        bitar[xtra + 0] = true;

                    }
                    else if (i == path.Count - 1) //end represented as blue
                    {
                        bitar[xtra + 3] = false;
                        bitar[xtra + 2] = true;
                        bitar[xtra + 1] = false;
                        bitar[xtra + 0] = false;
                    }
                    else //green
                    {
                        bitar[xtra + 3] = true;
                        bitar[xtra + 2] = false;
                        bitar[xtra + 1] = true;
                        bitar[xtra + 0] = false;
                    }
                    rgbValues[pos] = (byte)GetIntFromBitArray(bitar);
                }

                Marshal.Copy(rgbValues, 0, pointer, bytes);
                objBmpImage.UnlockBits(bmpData);
                objBmpImage.Save(fileName, imageFormat);
            }
        }


        /// <summary>
        /// This method gets a coolection of Walls and fill up our inner map
        /// </summary>
        /// <param name="walls"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Maze Load(List<Wall> walls, int width, int height)
        {
            Maze m = new Maze(width, height);

            for (int y = 0; y < m.Height - 1; y++) //TODO remove -1
            {
                for (int x = 0; x < m.Width - 1; x++)
                {
                    m.Map[x, y] = true;
                }
            }

            foreach (var wall in walls)
            {
                if (wall.yStart == wall.yEnd)
                {
                    for (int x = wall.xStart; x <= wall.xEnd; x++)
                    {
                        m.Map[x, wall.yStart] = false;
                    }
                }
                else
                {
                    for (int y = wall.yStart; y <= wall.yEnd; y++)
                    {
                        m.Map[wall.xStart, y] = false;
                    }
                }
            }

            return m;
        }





    }

    public struct MPoint
    {
        public int X, Y;
        public MPoint(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public abstract class Map
    {
        private int _width;
        private int _height;

        public int Width
        {
            get { return _width; }
        }


        public int Height
        {
            get { return _height; }
        }

        public Map(int width, int height)
        {
            this._width = width;
            this._height = height;
        }

        public abstract Boolean this[int x, int y] { get; set; }
    }

    public class InnerMap : Map
    {
        private InnerMapArray[] innerData;

        public InnerMap(int width, int height)
            : base(width, height)
        {
            innerData = new InnerMapArray[width];

            for (int i = 0; i < width; i++)
                innerData[i] = new InnerMapArray(height);
        }

        public override Boolean this[int x, int y]
        {
            get
            {
                return innerData[x][y];
            }
            set
            {
                innerData[x][y] = value;
            }
        }

    }

    class DotNetBitArrayInnerMap : InnerMap
    {
        private BitArray[] innerData;

        public DotNetBitArrayInnerMap(int width, int height)
            : base(width, height)
        {
            innerData = new BitArray[width];
            for (int i = 0; i < width; i++)
            {
                innerData[i] = new BitArray(height);
            }
        }

        public override Boolean this[int x, int y]
        {
            get
            {
                return innerData[x][y];
            }
            set
            {
                innerData[x][y] = value;
            }
        }
    }


    public class InnerMapArray
    {
        internal int[] innerData;

        public InnerMapArray(int height)
        {
            innerData = new int[height / 32 + 1];
        }

        public bool this[int y]
        {
            set
            {
                if (value)
                {
                    int a = 1 << y;
                    innerData[y / 32] |= a;
                }
                else
                {
                    int a = ~(1 << y);
                    innerData[y / 32] &= a;
                }
            }
            get
            {
                return (innerData[y / 32] & (1 << y)) != 0;
            }
        }
    }




}