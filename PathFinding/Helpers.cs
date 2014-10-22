using System;
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
    public class Helpers
    {
       
        public static List<Pixel> GetPixelListFromBitmap(Bitmap sourceImage)
        {
            BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0,
                        sourceImage.Width, sourceImage.Height),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] sourceBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, sourceBuffer.Length);
            sourceImage.UnlockBits(sourceData);

            var pixelList = new List<Pixel>(sourceBuffer.Length / 4);

            using (MemoryStream memoryStream = new MemoryStream(sourceBuffer))
            {
                memoryStream.Position = 0;
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                while (memoryStream.Position + 4 <= memoryStream.Length)
                {
                    Pixel pixel = new Pixel(binaryReader.ReadBytes(4));
                    pixelList.Add(pixel);
                }
                binaryReader.Close();
            }

            return pixelList;
        }
    }

    public class Pixel
    {
        public byte blue = 0;
        public byte green = 0;
        public byte red = 0;
        public byte alpha = 0;

        public Pixel()
        {

        }

        public Pixel(byte[] colorComponents)
        {
            blue = colorComponents[0];
            green = colorComponents[1];
            red = colorComponents[2];
            alpha = colorComponents[3];
        }

        public byte[] GetColorBytes()
        {
            return new byte[] { blue, green, red, alpha };
        }
    }
}
