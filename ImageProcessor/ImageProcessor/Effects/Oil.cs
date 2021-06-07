using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Effects
{
    class Oil
    {
        private int Radius { get { return 5; } }
        private int Levels { get { return 20; } }

        public BitmapSource Process(BitmapSource source)
        {
            var bitmap = new WriteableBitmap(source);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var stride = bitmap.BackBufferStride;
            var pixels = new byte[height * stride];
            var radiusOffset = (Radius - 1) / 2;
            bitmap.CopyPixels(pixels, stride, 0);

            for (int offsetY = radiusOffset; offsetY < height - radiusOffset; offsetY++)
            {
                for (int offsetX = radiusOffset; offsetX < width - radiusOffset; offsetX++)
                {
                    byte blue = 0;
                    byte green = 0;
                    byte red = 0;
                    var currentIntensity = 0;
                    var maxIntensity = 0;
                    var maxIndex = 0;
                    var intensityBin = new int[Levels + 1];
                    var blueBin = new int[Levels + 1];
                    var greenBin = new int[Levels + 1];
                    var redBin = new int[Levels + 1];
                    var byteOffset = offsetY * stride + offsetX * 4;

                    for (int filterY = -radiusOffset; filterY <= radiusOffset; filterY++)
                    {
                        for (int filterX = -radiusOffset; filterX <= radiusOffset; filterX++)
                        {
                            var calcOffset = byteOffset + (filterX * 4) + (filterY * stride);

                            currentIntensity = (int)Math.Round((pixels[calcOffset] + pixels[calcOffset + 1] + pixels[calcOffset + 2]) / 3.0 * (Levels) / 255.0);
                            intensityBin[currentIntensity] += 1;
                            blueBin[currentIntensity] += pixels[calcOffset];
                            greenBin[currentIntensity] += pixels[calcOffset + 1];
                            redBin[currentIntensity] += pixels[calcOffset + 2];

                            if (intensityBin[currentIntensity] > maxIntensity)
                            {
                                maxIntensity = intensityBin[currentIntensity];
                                maxIndex = currentIntensity;
                            }
                        }
                    }

                    blue = (byte)(blueBin[maxIndex] / maxIntensity);
                    green = (byte)(greenBin[maxIndex] / maxIntensity);
                    red = (byte)(redBin[maxIndex] / maxIntensity);

                    pixels[byteOffset] = blue;
                    pixels[byteOffset + 1] = green;
                    pixels[byteOffset + 2] = red;
                    pixels[byteOffset + 3] = 255;
                }
            }


            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
