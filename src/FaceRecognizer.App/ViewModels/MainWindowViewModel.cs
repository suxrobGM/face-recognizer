using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DlibDotNet;
using DlibDotNet.Extensions;
using Microsoft.Win32;
using FaceRecognizer.App.Core.Commands;
using FaceRecognizer.App.Core.Helpers;

namespace FaceRecognizer.App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            SelectImageCommand = new RelayCommand(_ => OpenFileDialog());
            RecognizeCommand = new RelayCommand(_ => Recognize(),
                _ => CanExecuteRecognizeCommand());
        }

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
                _imagePath = value;
                RaisePropertyChanged();
                RecognizeCommand.CanExecute(null);
            }
        }

        private BitmapSource _recognizedImageSource;
        public BitmapSource RecognizedImageSource
        {
            get => _recognizedImageSource;
            set
            {
                _recognizedImageSource = value;
                RaisePropertyChanged();
            }
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
            var img = Dlib.LoadImage<RgbPixel>(ImagePath);
            var faceDetector = Dlib.GetFrontalFaceDetector();
            var faces = faceDetector.Operator(img);

            foreach (var face in faces)
            {
                Dlib.DrawRectangle(img, face, new RgbPixel(0, 255, 255), 4);
            }

            RecognizedImageSource = ImageHelper.ConvertToBitmapSource(img.ToBitmap());
        }

        private bool CanExecuteRecognizeCommand() => !string.IsNullOrWhiteSpace(ImagePath);

        #endregion


        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}