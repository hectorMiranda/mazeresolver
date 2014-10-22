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
    /// <summary>
    /// 
    /// </summary>
    public class Maze
    {
        internal Map map;
        public int Width
        {
            get { return map.Width; }
        }
        public int Height
        {
            get { return map.Height; }
        }
        public Map Map
        {
            get { return map; }
        }
        public Maze(Bitmap image)
        {
            map = new Map(image.Width, image.Height);
            LoadBitmapAsMap(image);
        }

        public Maze(int width, int height)
        {
            map = new Map(width, height);
        }
       
        /// <summary>
        /// Allow us to save as a file based on the current content of map.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="imageFormat"></param>
        public void Save(String fileName, ImageFormat imageFormat)
        {
            using (Bitmap bitmap = new Bitmap(map.Width, map.Height, PixelFormat.Format1bppIndexed))
            {

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                var bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
                var rgbValues = new byte[bytes];

                Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int y = 0; y < map.Height; y++)
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

        public void Save(String fileName, ImageFormat imageFormat, List<MPoint> path)
        {

            using (Bitmap objBmpImage = new Bitmap(map.Width-1, map.Height-1, PixelFormat.Format4bppIndexed))
            {
                Rectangle rect = new Rectangle(0, 0, objBmpImage.Width, objBmpImage.Height);
                BitmapData bmpData = objBmpImage.LockBits(rect, ImageLockMode.ReadWrite, objBmpImage.PixelFormat);
                IntPtr pointer = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * objBmpImage.Height;
                byte[] rgbValues = new byte[bytes];

                Marshal.Copy(pointer, rgbValues, 0, bytes);

                for (int y = 0; y < map.Height-1; y++)
                {
                    int counter = bmpData.Stride * y;
                    int x = 0;
                    for (int i = 0; i < bmpData.Stride; i++)
                    {
                        if (x >= objBmpImage.Width)
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

        private void LoadBitmapAsMap(Bitmap bmp)
        {
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

    /// <summary>
    /// First attemp to represent the map
    /// </summary>
    public class Map
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

        private MapArray[] innerData;

        public Map(int width, int height)
        {
            this._width = width;
            this._height = height;
            innerData = new MapArray[width];

            for (int i = 0; i < width; i++)
                innerData[i] = new MapArray(height);
        }

        public Boolean this[int x, int y]
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

    internal class MapArray
    {
        internal int[] data;

        public MapArray(int height)
        {
            data = new int[height / 32 + 1];
        }

        public bool this[int y]
        {
            set
            {
                if (value)
                {
                    int a = 1 << y;
                    data[y / 32] |= a;
                }
                else
                {
                    int a = ~(1 << y);
                    data[y / 32] &= a;
                }
            }
            get
            {
                return (data[y / 32] & (1 << y)) != 0;
            }
        }
    }




}