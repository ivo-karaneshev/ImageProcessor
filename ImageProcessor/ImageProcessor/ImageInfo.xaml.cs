using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for ImageInfo.xaml
    /// </summary>
    public partial class ImageInfo : Window
    {
        public ImageInfo(BitmapSource source)
        {
            InitializeComponent();
            var bitmap = new WriteableBitmap(source);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var stride = bitmap.BackBufferStride;
            var size = height * stride;

            widthLbl.Content = $"Width: {width} px";
            heightLbl.Content = $"Height: {height} px";
            formatLbl.Content = $"Format: {bitmap.Format.ToString()}";
            dpiXLbl.Content = $"Width DPI: {bitmap.DpiX}";
            dpiYLbl.Content = $"Height DPI: {bitmap.DpiY}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
