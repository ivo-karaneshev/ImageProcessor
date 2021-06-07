using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Filters
{
    class Red
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
                pixels[i] = 0;
                pixels[i + 1] = 0;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
