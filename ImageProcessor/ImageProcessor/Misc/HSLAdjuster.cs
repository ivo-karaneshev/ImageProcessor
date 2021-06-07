using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor.Misc
{
    class HSLAdjuster
    {
        private double Hue { get; set; }
        private double Saturation { get; set; }
        private double Lightness { get; set; }

        public HSLAdjuster(int hue, int saturation, int lightness)
        {
            // Convert input values to decimal
            Hue = hue / 360d;
            Saturation = saturation / 100d;
            Lightness = lightness / 100d;
        }

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
                var hsl = RgbToHsl(pixels[i + 2], pixels[i + 1], pixels[i]);

                // Hue
                var hue = hsl[0] + Hue;
                while (hue < 0) { hue += 1; }
                while (hue > 1) { hue -= 1; }

                // Saturation
                var saturation = hsl[1] + hsl[1] * Saturation;
                if (saturation < 0) { saturation = 0; }
                else if (saturation > 1) { saturation = 1; }

                // Lightness
                var lightness = hsl[2];
                if (Lightness > 0) { lightness += (1 - lightness) * Lightness; }
                else if (Lightness < 0) { lightness += lightness * Lightness; }

                var rgb = HslToRgb(hue, saturation, lightness);
                pixels[i] = rgb[2];
                pixels[i + 1] = rgb[1];
                pixels[i + 2] = rgb[0];
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(rect, pixels, stride, 0);
            return bitmap;
        }

        private double[] RgbToHsl(byte red, byte green, byte blue)
        {
            double r = red / 255d;
            double g = green / 255d;
            double b = blue / 255d;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chroma = max - min;
            double h = 0;
            double s = 0;
            // Lightness
            double l = (min + max) / 2d;

            if (chroma != 0)
            {
                // Hue
                if (r == max)
                {
                    h = (g - b) / chroma + ((g < b) ? 6d : 0);
                }
                else if (g == max)
                {
                    h = (b - r) / chroma + 2d;
                }
                else
                {
                    h = (r - g) / chroma + 4d;
                }
                h /= 6d;

                // Saturation
                s = (l > 0.5) ? chroma / (2d - max - min) : chroma / (max + min);
            }

            return new double[] { h, s, l };
        }

        private byte[] HslToRgb(double h, double s, double l)
        {
            double m1 = 0, m2 = 0, hue = 0;
            byte r = 0, g = 0, b = 0;
            byte[] rgb = new byte[3];

            if (s == 0)
            {
                r = g = b = (byte)(l * 255 + 0.5);
                rgb[0] = r;
                rgb[1] = g;
                rgb[2] = b;
            }
            else
            {
                if (l <= 0.5)
                {
                    m2 = l * (s + 1);
                }
                else
                {
                    m2 = l + s - l * s;
                }

                m1 = l * 2 - m2;
                hue = h + 1 / 3d;

                double tmp = 0;
                for (var i = 0; i < 3; i += 1)
                {
                    if (hue < 0)
                    {
                        hue += 1;
                    }
                    else if (hue > 1)
                    {
                        hue -= 1;
                    }

                    if (6 * hue < 1)
                    {
                        tmp = m1 + (m2 - m1) * hue * 6d;
                    }
                    else if (2d * hue < 1)
                    {
                        tmp = m2;
                    }
                    else if (3d * hue < 2)
                    {
                        tmp = m1 + (m2 - m1) * (2d / 3d - hue) * 6d;
                    }
                    else
                    {
                        tmp = m1;
                    }

                    rgb[i] = (byte)(tmp * 255 + 0.5);

                    hue -= 1d / 3d;
                }
            }

            return rgb;
        }
    }
}
