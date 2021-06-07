using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Filters
{
    class Grayscale
    {
        public BitmapSource Process(BitmapSource source)
        {
            var bitmap = new WriteableBitmap(source);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var stride = bitmap.BackBufferStride;
            var pixels = new byte[height * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (var i = 0; i < pixels.Length; i += 4)
            {
                var blue = pixels[i];
                var green = pixels[i + 1];
                var red = pixels[i + 2];

                var grayscale = (byte)(red * 0.3 + green * 0.59 + blue * 0.11);
                pixels[i] = grayscale;
                pixels[i + 1] = grayscale;
                pixels[i + 2] = grayscale;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
