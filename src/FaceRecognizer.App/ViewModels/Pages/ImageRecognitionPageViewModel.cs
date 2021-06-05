using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DlibDotNet;
using DlibDotNet.Extensions;
using Microsoft.Win32;
using FaceRecognizer.App.Core.Commands;
using FaceRecognizer.App.Core.Helpers;

namespace FaceRecognizer.App.ViewModels.Pages
{
    public class ImageRecognitionPageViewModel : ViewModelBase
    {
        #region Ctor

        public ImageRecognitionPageViewModel()
        {
            SelectImageCommand = new RelayCommand(_ => OpenFileDialog());
            RecognizeCommand = new RelayCommand(_ => Recognize(),
                _ => CanExecuteRecognizeCommand());
        }

        #endregion
        

        #region Commands

        public ICommand SelectImageCommand { get; }
        public ICommand RecognizeCommand { get; }

        #endregion

        #region Bindable Properties

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                SetProperty(ref _imagePath, value);
                RecognizeCommand.CanExecute(null);
            }
        }

        private BitmapSource _recognizedImageSource;
        public BitmapSource RecognizedImageSource
        {
            get => _recognizedImageSource;
            set => SetProperty(ref _recognizedImageSource, value);
        }

        #endregion


        #region Methods

        private void OpenFileDialog()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false, 
                Filter = "Image files (.png, .jpg)|*.png;*.jpg"
            };
            fileDialog.ShowDialog();
            ImagePath = fileDialog.FileName;
        }

        private void Recognize()
        {
            const string label = "Sukhrob";
            var rectBrush = new SolidBrush(Color.FromArgb(0, 255, 255));

            using var img = Dlib.LoadImage<RgbPixel>(ImagePath);
            using var bitmap = img.ToBitmap();
            using var graphics = Graphics.FromImage(bitmap);
            using var font = new Font(SystemFonts.DefaultFont.FontFamily, 54, FontStyle.Bold);
            using var pen = new Pen(rectBrush)
            {
                Width = 5
            };

            var faceDetector = Dlib.GetFrontalFaceDetector();
            var faces = faceDetector.Operator(img);

            foreach (var face in faces)
            {
                var rect = new System.Drawing.Rectangle()
                {
                    X = face.TopLeft.X,
                    Y = face.TopLeft.Y,
                    Width = (int)face.Width,
                    Height = (int)face.Height
                };
                graphics.DrawRectangle(pen, rect);

                // draw string with background
                var namePos = new PointF(face.BottomLeft.X - 150, face.BottomLeft.Y + 10);
                var strSize = graphics.MeasureString(label, font);
                var strBackground = new System.Drawing.Rectangle()
                {
                    Location = new System.Drawing.Point((int)namePos.X, (int)namePos.Y),
                    Size = new Size((int) strSize.Width, (int) strSize.Height)
                };
                graphics.FillRectangle(Brushes.Chartreuse, strBackground);
                graphics.DrawString(label, font, Brushes.Red, namePos);
            }

            RecognizedImageSource = ImageHelper.ConvertToBitmapSource(bitmap);

            //using var img = FaceRecognition.LoadImageFile(ImagePath);
            //var faceRecognition = FaceRecognition.Create("output");
            //var faceLocations = faceRecognition.FaceLocations(img);
        }

        private bool CanExecuteRecognizeCommand() => !string.IsNullOrWhiteSpace(ImagePath);

        #endregion
    }
}