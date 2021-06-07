using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Effects
{
    class Pixelate
    {
        private int PixelateSize { get { return 8; } }

        public BitmapSource Process(BitmapSource source)
        {
            var bitmap = new WriteableBitmap(source);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var stride = bitmap.BackBufferStride;
            var pixels = new byte[height * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (var xx = 0; xx < width; xx += PixelateSize)
            {
                for (var yy = 0; yy < height; yy += PixelateSize)
                {
                    var offsetX = PixelateSize / 2;
                    var offsetY = PixelateSize / 2;

                    while (xx + offsetX >= width) offsetX--;
                    while (yy + offsetY >= height) offsetY--;

                    var index = (xx + offsetX) * 4 + (yy + offsetY) * stride;
                    var blue = pixels[index];
                    var green = pixels[index + 1];
                    var red = pixels[index + 2];

                    for (var x = xx; x < xx + PixelateSize && x < width; x++)
                    {
                        for (var y = yy; y < yy + PixelateSize && y < height; y++)
                        {
                            index = y * stride + x * 4;
                            pixels[index] = blue;
                            pixels[index + 1] = green;
                            pixels[index + 2] = red;
                        }
                    }
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
