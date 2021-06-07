using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Misc
{
    class Fill
    {
        public BitmapSource Process(BitmapSource source, byte red, byte green, byte blue)
        {
            var bitmap = new WriteableBitmap(source);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var stride = bitmap.BackBufferStride;
            var pixels = new byte[height * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (var i = 0; i < pixels.Length; i += 4)
            {
                if (pixels[i] < blue)
                {
                    pixels[i] = blue;
                }

                if (pixels[i + 1] < green)
                {
                    pixels[i + 1] = green;
                }

                if (pixels[i + 2] < red)
                {
                    pixels[i + 2] = red;
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }
    }
}
