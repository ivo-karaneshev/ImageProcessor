using ImageProcessor.Misc;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for HSL.xaml
    /// </summary>
    public partial class HSL : Window
    {
        private BitmapSource original;

        public HSL(BitmapSource source)
        {
            InitializeComponent();
            original = Resize(source);
            imagePreview.Source = original;
        }

        private BitmapSource Resize(BitmapSource source)
        {
            var maxSide = Math.Max(source.PixelWidth, source.PixelHeight);
            var baseSide = 500d;
            if (maxSide > baseSide)
            {
                var scale = baseSide / maxSide;
                source = new TransformedBitmap(source, new ScaleTransform(scale, scale));
            }

            return source;
        }

        private void HslHandler(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var h = (int)hue.Value;
            var s = (int)saturation.Value;
            var l = (int)lightness.Value;

            var result = new HSLAdjuster(h, s, l).Process(original);
            imagePreview.Source = result;
        }
    }
}
