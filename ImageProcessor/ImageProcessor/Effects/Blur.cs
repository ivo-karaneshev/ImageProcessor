using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Effects
{
    class Blur
    {
        private int Radius { get { return 8; } }

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
                    var blue = (byte)0;
                    var green = (byte)0;
                    var red = (byte)0;
                    var byteOffset = offsetY * stride + offsetX * 4;
                    var avgR = 0;
                    var avgG = 0;
                    var avgB = 0;
                    var blurPixelCount = 0;

                    for (int filterY = -radiusOffset; filterY <= radiusOffset; filterY++)
                    {
                        for (int filterX = -radiusOffset; filterX <= radiusOffset; filterX++)
                        {
                            var calcOffset = byteOffset + (filterX * 4) + (filterY * stride);

                            avgB += pixels[calcOffset];
                            avgG += pixels[calcOffset + 1];
                            avgR += pixels[calcOffset + 2];

                            blurPixelCount++;
                        }
                    }

                    blue = (byte)(avgB / blurPixelCount);
                    green = (byte)(avgG / blurPixelCount);
                    red = (byte)(avgR / blurPixelCount);

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
