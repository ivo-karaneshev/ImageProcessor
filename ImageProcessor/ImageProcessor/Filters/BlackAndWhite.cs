using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Filters
{
    class BlackAndWhite
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

                var intensity = (red + green + blue) / 3d;
                var color = (byte)(intensity > 255 / 2d ? 255 : 0);
                pixels[i] = color;
                pixels[i + 1] = color;
                pixels[i + 2] = color;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
