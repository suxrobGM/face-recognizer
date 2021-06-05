using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Serilog;
using FaceRecognizer.App.Core.Commands;
using FaceRecognizer.App.Core.Helpers;

namespace FaceRecognizer.App.ViewModels.Pages
{
    public class RealTimeRecognitionPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly DispatcherTimer _timer;
        private VideoCapture _capture;
        private CascadeClassifier _faceClassifier;

        #endregion

        #region Ctor

        public RealTimeRecognitionPageViewModel()
        {
            StartCameraCommand = new RelayCommand(_ => StartCamera(),
                _ => CanExecuteStartCameraCommand());
            StopCameraCommand = new RelayCommand(_ => StopCamera(),
                _ => CanExecuteStopCameraCommand());

            AvailableCameras = new ObservableCollection<string>(GetAllConnectedCameras());
            SelectedCamera = AvailableCameras.FirstOrDefault();
            CanChangeCamera = true;

            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30) // 30 fps
            };
            _timer.Tick += TimerOnTick;
        }

        #endregion

        #region Commands

        public ICommand StartCameraCommand { get; }
        public ICommand StopCameraCommand { get; }

        #endregion

        #region Observable collections

        public ObservableCollection<string> AvailableCameras { get; }

        #endregion

        #region Bindable properties

        private string _selectedCamera;

        public string SelectedCamera
        {
            get => _selectedCamera;
            set => SetProperty(ref _selectedCamera, value);
        }


        private bool _canChangeCamera;

        public bool CanChangeCamera
        {
            get => _canChangeCamera;
            set
            {
                SetProperty(ref _canChangeCamera, value);

                // Raise can execute methods
                StartCameraCommand.CanExecute(null);
                StopCameraCommand.CanExecute(null);
            }  
        }

        private BitmapSource _currentFrame;

        public BitmapSource CurrentFrame
        {
            get => _currentFrame;
            set => SetProperty(ref _currentFrame, value);
        }

        #endregion

        #region Methods

        #region Private methods

        private void StartCamera()
        {
            if (string.IsNullOrEmpty(SelectedCamera))
            {
                Log.Logger?.Warning("Did not selected camera");
                MessageBox.Show("Please select camera", "Attention", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
                

            var cascadeFile = Path.Combine(Directory.GetCurrentDirectory(), "Data",
                "haarcascade_frontalface_default.xml");

            if (!File.Exists(cascadeFile))
            {
                Log.Logger?.Error("Could not found pre trained cascade models file");
                MessageBox.Show("Could not found pre trained cascade models file", "ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var camIndex = AvailableCameras.IndexOf(SelectedCamera);
            _capture = new VideoCapture(camIndex);
            _faceClassifier = new CascadeClassifier(cascadeFile);
            _timer.Start();
            CanChangeCamera = false;
        }

        private void StopCamera()
        {
            _capture.Dispose();
            _faceClassifier.Dispose();
            _timer.Stop();
            CanChangeCamera = true;
        }

        private bool CanExecuteStartCameraCommand() => CanChangeCamera && !string.IsNullOrEmpty(SelectedCamera);
        private bool CanExecuteStopCameraCommand() => !CanChangeCamera;

        #endregion


        #region Event handlers

        private void TimerOnTick(object sender, EventArgs e)
        {
            try
            {
                using var frame = _capture.QueryFrame();
                if (frame != null)
                {
                    DetectFaces(frame);
                    CurrentFrame = ImageHelper.ConvertToBitmapSource(frame.ToBitmap());
                }
            }
            catch (Exception ex)
            {
                Log.Logger?.Error("Thrown exception on capturing video: {Exception}", ex);
            }
        }

        private void DetectFaces(IInputOutputArray frame)
        {
            var faces = _faceClassifier.DetectMultiScale(frame, 1.1, 5);

            foreach (var face in faces)
            {
                CvInvoke.Rectangle(frame, face, new MCvScalar(255, 255, 0), 3);
            }
        }

        #endregion

        #region Static methods

        public static IEnumerable<string> GetAllConnectedCameras()
        {
            var cameraNames = new List<string>();
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')");
            foreach (var device in searcher.Get())
            {
                cameraNames.Add(device["Caption"].ToString());
            }

            return cameraNames;
        }

        #endregion

        #endregion
    }
}