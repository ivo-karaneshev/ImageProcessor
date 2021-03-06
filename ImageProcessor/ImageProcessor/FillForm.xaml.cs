using ImageProcessor.Misc;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for Fill.xaml
    /// </summary>
    public partial class FillForm : Window
    {
        private BitmapSource original;

        public FillForm(BitmapSource source)
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

        private void FillHandler(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var result = new Fill().Process(original, (byte)red.Value, (byte)green.Value, (byte)blue.Value);
            imagePreview.Source = result;
        }
    }
}
