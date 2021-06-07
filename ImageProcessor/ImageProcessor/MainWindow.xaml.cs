using ImageProcessor.Common;
using ImageProcessor.Effects;
using ImageProcessor.Filters;
using ImageProcessor.Misc;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource original;
        private BitmapSource current;
        private bool isSaved = true;
        private Stack<BitmapSource> undoHistory = new Stack<BitmapSource>();
        private Stack<BitmapSource> redoHistory = new Stack<BitmapSource>();

        public MainWindow()
        {
            InitializeComponent();
            CreateNew();
            CreateShortcuts();
        }

        private void SetCurrent(BitmapSource source)
        {
            undoHistory.Push(current);
            redoHistory.Clear();
            current = source;
            image.Source = current;
            isSaved = false;

            // UI Logic
            resetImageBtn.IsEnabled = true;
            toggleImageBtn.IsEnabled = true;
            undoBtn.IsEnabled = true;
            redoBtn.IsEnabled = false;
        }

        private void CreateNew()
        {
            if (isSaved || MessageBox.Show("You have unsaved progress that will be lost if you exit. Continue anyway?",
                "New", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var blank = new Blank().Create();
                original = blank;
                current = original;
                image.Source = current;
                isSaved = true;
                undoHistory.Clear();
                redoHistory.Clear();

                // UI Logic
                resetImageBtn.IsEnabled = false;
                toggleImageBtn.IsEnabled = false;
                undoBtn.IsEnabled = false;
                redoBtn.IsEnabled = false;
            }
        }

        #region Handlers

        private void CopyHandler(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(current);
        }

        private void PasteHandler(object sender, RoutedEventArgs e)
        {
            var source = Clipboard.GetImage();

            if (source != null)
            {
                SetCurrent(source);
            }
        }

        private void UndoHandler(object sender, RoutedEventArgs e)
        {
            if (!undoBtn.IsEnabled)
            {
                return;
            }

            if (undoHistory.Count > 0)
            {
                var previous = undoHistory.Pop();
                redoHistory.Push(current);
                current = previous;
                image.Source = current;

                if (undoHistory.Count == 0)
                {
                    // This means that current and original are equal
                    isSaved = true;

                    // UI Logic
                    undoBtn.IsEnabled = false;
                    toggleImageBtn.IsEnabled = false;
                    resetImageBtn.IsEnabled = false;
                }

                // UI Logic
                redoBtn.IsEnabled = true;
            }
        }

        private void RedoHandler(object sender, RoutedEventArgs e)
        {
            if (!redoBtn.IsEnabled)
            {
                return;
            }

            if (redoHistory.Count > 0)
            {
                var next = redoHistory.Pop();
                undoHistory.Push(current);
                current = next;
                image.Source = current;
                isSaved = false;

                if (redoHistory.Count == 0)
                {
                    // UI Logic
                    redoBtn.IsEnabled = false;
                }

                // UI Logic
                undoBtn.IsEnabled = true;
                toggleImageBtn.IsEnabled = true;
                resetImageBtn.IsEnabled = true;
            }
        }

        private void NewHandler(object sender, RoutedEventArgs e)
        {
            CreateNew();
        }

        private void LoadImageHandler(object sender, RoutedEventArgs e)
        {
            if (isSaved || MessageBox.Show("You have unsaved progress that will be lost if you exit. Continue anyway?",
                "Load Image", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    original = new BitmapImage(new Uri(openFileDialog.FileName));
                    current = original;
                    image.Source = current;
                    isSaved = true;
                    undoHistory.Clear();
                    redoHistory.Clear();

                    //UI Logic
                    resetImageBtn.IsEnabled = false;
                    toggleImageBtn.IsEnabled = false;
                    undoBtn.IsEnabled = false;
                    redoBtn.IsEnabled = false;
                }
            }
        }

        private void SaveImageHandler(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG File (*.jpg)|*.jpg|PNG File (*.png)|*.png|BMP File (*.bmp)|*.bmp"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var extension = Path.GetExtension(saveFileDialog.FileName);
                if (extension.Equals(Constants.JPEGFileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(current));

                    using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
                else if (extension.Equals(Constants.PNGFileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(current));

                    using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
                else if (extension.Equals(Constants.BMPFileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(current));

                    using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                isSaved = true;
            }
        }

        private void ExitHandler(object sender, RoutedEventArgs e)
        {
            if (isSaved || MessageBox.Show("You have unsaved progress that will be lost if you exit. Continue anyway?",
                    "Exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void WindowCloseHandler(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (isSaved || MessageBox.Show("You have unsaved progress that will be lost if you exit. Continue anyway?",
                    "Exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
        }

        private void ToggleImageHandler(object sender, RoutedEventArgs e)
        {
            if (!toggleImageBtn.IsEnabled)
            {
                return;
            }

            if (image.Source == original)
            {
                image.Source = current;
            }
            else
            {
                image.Source = original;
            }
        }

        private void ResetHandler(object sender, RoutedEventArgs e)
        {
            if (!resetImageBtn.IsEnabled)
            {
                return;
            }

            if (isSaved || MessageBox.Show("You have unsaved progress that will be lost if you reset. Continue anyway?",
                    "Reset", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                current = original;
                image.Source = current;
                isSaved = true;
                undoHistory.Clear();
                redoHistory.Clear();

                // UI Logic
                toggleImageBtn.IsEnabled = false;
                resetImageBtn.IsEnabled = false;
                undoBtn.IsEnabled = false;
                redoBtn.IsEnabled = false;
            }
        }

        private void FiltersHandler(object sender, RoutedEventArgs e)
        {
            BitmapSource result = current;

            if (sender == sepiaBtn)
            {
                result = new Sepia().Process(current);
            }
            else if (sender == grayscaleBtn)
            {
                result = new Grayscale().Process(current);
            }
            else if (sender == redBtn)
            {
                result = new Red().Process(current);
            }
            else if (sender == greenBtn)
            {
                result = new Green().Process(current);
            }
            else if (sender == blueBtn)
            {
                result = new Blue().Process(current);
            }
            else if (sender == invertBtn)
            {
                result = new Invert().Process(current);
            }
            else if (sender == blackAndWhiteBtn)
            {
                result = new BlackAndWhite().Process(current);
            }

            SetCurrent(result);
        }

        private void EffectsHandler(object sender, RoutedEventArgs e)
        {
            BitmapSource result = current;

            if (sender == ditherBtn)
            {
                result = new Dither().Process(current);
            }
            else if (sender == colorDitherBtn)
            {
                result = new ColorDither().Process(current);
            }
            else if (sender == pixelateBtn)
            {
                result = new Pixelate().Process(current);
            }
            else if (sender == oilBtn)
            {
                result = new Oil().Process(current);
            }
            else if (sender == blurBtn)
            {
                result = new Blur().Process(current);
            }

            SetCurrent(result);
        }

        private void OpenFillFormHandler(object sender, RoutedEventArgs e)
        {
            var form = new FillForm(current)
            {
                Owner = GetWindow(this)
            };

            form.applyBtn.Click += (_sender, _e) => FillHandler(_sender, _e, form);
            form.Show();
        }

        private void FillHandler(object sender, RoutedEventArgs e, FillForm form)
        {
            var result = new Fill().Process(current, (byte)form.red.Value, (byte)form.green.Value, (byte)form.blue.Value);
            SetCurrent(result);

            // Close Fill window
            form.Close();
        }

        private void OpenImageInfoFormHandler(object sender, RoutedEventArgs e)
        {
            var form = new ImageInfo(current)
            {
                Owner = GetWindow(this)
            };

            form.Show();
        }

        private void OpenHslFormHandler(object sender, RoutedEventArgs e)
        {
            var form = new HSL(current)
            {
                Owner = GetWindow(this)
            };

            form.applyBtn.Click += (_sender, _e) => HlsHandler(_sender, _e, form);
            form.Show();
        }

        private void HlsHandler(object sender, RoutedEventArgs e, HSL form)
        {
            var h = (int)form.hue.Value;
            var s = (int)form.saturation.Value;
            var l = (int)form.lightness.Value;

            var result = new HSLAdjuster(h, s, l).Process(current);
            SetCurrent(result);

            // Close HLS window
            form.Close();
        }


        #endregion

        #region Shortcuts

        private void CreateShortcuts()
        {
            // New image CTRL + N
            var newCmd = new RoutedCommand();
            newCmd.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newCmd, NewHandler));

            // Open image CTRL + O
            var openCmd = new RoutedCommand();
            openCmd.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(openCmd, LoadImageHandler));

            // Save image CTRL + S
            var saveCmd = new RoutedCommand();
            saveCmd.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(saveCmd, SaveImageHandler));

            // Exit CTRL + E
            var exitCmd = new RoutedCommand();
            exitCmd.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(exitCmd, ExitHandler));

            // Undo CTRL + Z
            var undoCmd = new RoutedCommand();
            undoCmd.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(undoCmd, UndoHandler));

            // Redo CTRL + SHIFT + Z
            var redoCmd = new RoutedCommand();
            redoCmd.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift));
            CommandBindings.Add(new CommandBinding(redoCmd, RedoHandler));

            // Copy CTRL + C
            var copyCmd = new RoutedCommand();
            copyCmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(copyCmd, CopyHandler));

            // Paste CTRL + V
            var pasteCmd = new RoutedCommand();
            pasteCmd.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(pasteCmd, PasteHandler));

            // Reset Image CTRL + R
            var resetCmd = new RoutedCommand();
            resetCmd.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(resetCmd, ResetHandler));

            // Toggle Image CTRL + T
            var toggleCmd = new RoutedCommand();
            toggleCmd.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(toggleCmd, ToggleImageHandler));

            // Image Info CTRL + I
            var infoCmd = new RoutedCommand();
            infoCmd.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(infoCmd, OpenImageInfoFormHandler));
        }

        #endregion
    }
}
