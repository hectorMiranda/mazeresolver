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
    /// A maze object which holds the map array data 
    /// exposing it as a public property.
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
        
        /// <summary>
        /// Allows the creation of a Maze object by reading the pixel values from the image
        /// </summary>
        /// <param name="image">A valid image file</param>
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
        /// <param name="fileName">Name for the output file</param>
        /// <param name="imageFormat">Supported formats include Bmp, Gif, png and all the other formats define in the ImageFormat enumeration </param>
        public void Save(String fileName, ImageFormat imageFormat)
        {
            using (Bitmap bitmap = new Bitmap(map.Width, map.Height, PixelFormat.Format1bppIndexed))
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

                IntPtr pointer = bmpData.Scan0;

                var bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
                var rgbValues = new byte[bytes];

                Marshal.Copy(pointer, rgbValues, 0, bytes);

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

                Marshal.Copy(rgbValues, 0, pointer, bytes);
                bitmap.UnlockBits(bmpData);
                bitmap.Save(fileName, imageFormat);
            }
        }

        /// <summary>
        /// Allow us to save as an image file based on calculated Path
        /// </summary>
        /// <param name="fileName">Name for the output file</param>
        /// <param name="imageFormat">Supported formats include Bmp, Gif, png and all the other formats define in the ImageFormat enumeration </param>
        /// <param name="path"> List of map points equivalent to the map array</param>
        public void Save(String fileName, ImageFormat imageFormat, List<MPoint> path)
        {

            using (Bitmap bitmap = new Bitmap(map.Width-1, map.Height-1, PixelFormat.Format4bppIndexed))
            {
                Rectangle frame = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bmpData = bitmap.LockBits(frame, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr pointer = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
                byte[] rgbValues = new byte[bytes];

                Marshal.Copy(pointer, rgbValues, 0, bytes);

                for (int y = 0; y < map.Height-1; y++)
                {
                    int counter = bmpData.Stride * y;
                    int x = 0;
                    for (int i = 0; i < bmpData.Stride; i++)
                    {
                        if (x >= bitmap.Width)
                        {
                            break;
                        }

                        BitArray bitArray = new BitArray(8);

                        for (int j = 4; j >= 0; j = j - 4)
                        {
                            if (map[x, y])
                            {
                                bitArray[j + 3] = true;
                                bitArray[j + 2] = true;
                                bitArray[j + 1] = true;
                                bitArray[j + 0] = true;
                            }
                            else
                            {
                                bitArray[j + 3] = false;
                                bitArray[j + 2] = false;
                                bitArray[j + 1] = false;
                                bitArray[j + 0] = false;
                            }
                            x++;
                        }
                        rgbValues[counter] = (byte)GetIntFromBitArray(bitArray);
                        counter++;
                    }
                }

                for (int i = 0; i < path.Count; i++)
                {
                    MPoint point = path[i];
                    int xrest = point.X % 2;

                    int pos = bmpData.Stride * point.Y + point.X / 2;

                    BitArray bitar = GetBitArrayFromByte(rgbValues[pos]);

                    int xtra = 4;

                    if (xrest >= 1)
                    {
                        xtra = 0;
                    }

                    //per requirement the start point is represented as red
                    if (i == 0)
                    {
                        bitar[xtra + 3] = true;
                        bitar[xtra + 2] = false;
                        bitar[xtra + 1] = false;
                        bitar[xtra + 0] = true;

                    }
                    else if (i == path.Count - 1) //per requirement the end point is represented as blue
                    {
                        bitar[xtra + 3] = false;
                        bitar[xtra + 2] = true;
                        bitar[xtra + 1] = false;
                        bitar[xtra + 0] = false;
                    }
                    else //per requirement solution will be highlited in green
                    {
                        bitar[xtra + 3] = true;
                        bitar[xtra + 2] = false;
                        bitar[xtra + 1] = true;
                        bitar[xtra + 0] = false;
                    }
                    rgbValues[pos] = (byte)GetIntFromBitArray(bitar);
                }

                Marshal.Copy(rgbValues, 0, pointer, bytes);
                bitmap.UnlockBits(bmpData);
                bitmap.Save(fileName, imageFormat);
            }
        }

        /// <summary>
        /// Set the values for the map in memory
        /// </summary>
        /// <remarks>This mapper will actually identify the blu and red points, needed by the pathfinder</remarks>
        /// <param name="image">A valid image</param>
        private void LoadBitmapAsMap(Bitmap image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var currentPixel = image.GetPixel(x, y);

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

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var firstPixel = image.GetPixel(x, y);

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

        private BitArray GetBitArrayFromByte(byte byteValue)
        {
            BitArray b = new BitArray(new byte[1] { byteValue });
            return b;
        }

    }

    /// <summary>
    /// Simple struct used to keep track of all the map points
    /// </summary>
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
    /// This class represents a Map object encapsulating all 
    /// the bit informaiton in a MapArray object
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
    /// <summary>
    /// Map Array object taking care of the bitwise operations.
    /// </summary>
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
                    data[y / 32] |= a; //bitwise inclusive 
                }
                else
                {
                    int a = ~(1 << y); //lets flip some bits
                    data[y / 32] &= a; //Bitwise AND 
                }
            }
            get
            {
                return (data[y / 32] & (1 << y)) != 0;
            }
        }
    }




}