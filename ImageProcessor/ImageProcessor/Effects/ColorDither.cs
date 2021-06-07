using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Effects
{
    class ColorDither
    {
        private byte[,] ditherMatrix = new byte[4, 4]
        {
            { 0, 8, 2, 10 },
            { 12, 4, 14, 6 },
            { 3, 11, 1, 9 },
            { 15, 7, 13, 5 }
        };

        private byte[] redPalette = new byte[] { 0, 51, 102, 153, 204, 255 };
        private byte[] greenPalette = new byte[] { 0, 42, 85, 127, 170, 212, 255 };
        private byte[] bluePalette = new byte[] { 0, 51, 102, 153, 204, 255 };


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
                    var factor = ditherMatrix[k, m];

                    pixels[index] = ReduceColor(blue + factor, bluePalette);
                    pixels[index + 1] = ReduceColor(green + factor, greenPalette);
                    pixels[index + 2] = ReduceColor(red + factor, redPalette);
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }

        private byte ReduceColor(int color, byte[] palette)
        {
            if (color < 0) { color = 0; }
            if (color > 255) { color = 255; }

            for (var i = 1; i < palette.Length; i++)
            {
                if (color >= palette[i - 1] && color < palette[i])
                {
                    color = palette[i - 1];
                    break;
                }
            }

            return (byte)color;
        }
    }
}
