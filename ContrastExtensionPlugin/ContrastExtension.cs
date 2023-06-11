using PluginBase;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ContrastExtensionPlugin
{
    public class ContrastExtension : ICommand
    {
        public string Name { get => "ContrastExtension"; }

        public string Description => throw new NotImplementedException();

        public double Version => throw new NotImplementedException();

        public Bitmap Execute(Bitmap sourceBitmap, double threshold)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                sourceBitmap.Width, sourceBitmap.Height),
                                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                double blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
                contrastLevel) + 0.5) * 255.0;


                double green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
                contrastLevel) + 0.5) * 255.0;


                double red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
                contrastLevel) + 0.5) * 255.0;


                if (blue > 255)
                { blue = 255; }
                else if (blue < 0)
                { blue = 0; }


                if (green > 255)
                { green = 255; }
                else if (green < 0)
                { green = 0; }


                if (red > 255)
                { red = 255; }
                else if (red < 0)
                { red = 0; }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }


            Bitmap resultBitmap = new(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                        resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }
    }
}
