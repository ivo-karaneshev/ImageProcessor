using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Effects
{
    class Dither
    {
        private byte[,] ditherMatrix = new byte[4, 4]
        {
            { 0, 8, 2, 10 },
            { 12, 4, 14, 6 },
            { 3, 11, 1, 9 },
            { 15, 7, 13, 5 }
        };

        private int Approximate(byte grayscale)
        {
            var length = ditherMatrix.Length;
            var step = 256 / length;
            var count = 0;
            for (var i = 0; i < 255; i += step)
            {
                if (grayscale >= i && grayscale < i + step)
                {
                    return length - 1 - count;
                }

                count++;
            }

            return 0;
        }

        public BitmapSource Process(BitmapSource source)
        {
            var bitmap = new WriteableBitmap(source);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var stride = bitmap.BackBufferStride;
            var pixels = new byte[height * stride];
            var size = ditherMatrix.GetLength(0);
            bitmap.CopyPixels(pixels, stride, 0);

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var index = i * stride + j * 4;
                    var blue = pixels[index];
                    var green = pixels[index + 1];
                    var red = pixels[index + 2];
                    var k = i % size;
                    var m = j % size;
                    var grayscale = (byte)(red * 0.3 + green * 0.59 + blue * 0.11);

                    if (Approximate(grayscale) < ditherMatrix[k, m])
                    {
                        pixels[index] = 255;
                        pixels[index + 1] = 255;
                        pixels[index + 2] = 255;
                    }
                    else
                    {
                        pixels[index] = 0;
                        pixels[index + 1] = 0;
                        pixels[index + 2] = 0;
                    }
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
