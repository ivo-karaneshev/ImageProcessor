using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageProcessor.Filters
{
    class Sepia
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
                var b = pixels[i];
                var g = pixels[i + 1];
                var r = pixels[i + 2];
                var tr = (int)(r * 0.393 + g * 0.769 + b * 0.189);
                var tg = (int)(r * 0.349 + g * 0.686 + b * 0.168);
                var tb = (int)(r * 0.272 + g * 0.534 + b * 0.131);

                byte red = 0;
                byte green = 0;
                byte blue = 0;

                SetColor(ref red, tr);
                SetColor(ref green, tg);
                SetColor(ref blue, tb);

                pixels[i] = blue;
                pixels[i + 1] = green;
                pixels[i + 2] = red;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }

        private void SetColor(ref byte color, int threshold)
        {
            if (threshold > 255)
            {
                color = 255;
            }
            else
            {
                color = (byte)threshold;
            }
        }
    }
}
