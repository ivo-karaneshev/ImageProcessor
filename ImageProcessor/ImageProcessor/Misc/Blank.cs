using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ImageProcessor.Misc
{
    class Blank
    {
        public BitmapSource Create()
        {
            var width = 1152;
            var height = 648;
            var stride = width * 4;
            var pixels = new byte[height * stride];

            var colors = new List<Color>();
            colors.Add(Colors.Blue);
            colors.Add(Colors.Green);
            colors.Add(Colors.Red);
            var myPalette = new BitmapPalette(colors);

            for (var i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 255;
                pixels[i + 1] = 255;
                pixels[i + 2] = 255;
            }

            var image = BitmapSource.Create(
                width, height,
                300, 300,
                PixelFormats.Bgr32,
                myPalette,
                pixels,
                stride);

            return image;
        }
    }
}
