using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
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
            if(string.IsNullOrEmpty(SelectedCamera))
                return;

            var camIndex = AvailableCameras.IndexOf(SelectedCamera);
            _capture = new VideoCapture(camIndex);
            _timer.Start();

            CanChangeCamera = false;
        }

        private void StopCamera()
        {
            _capture.Dispose();
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
                    CurrentFrame = ImageHelper.ConvertToBitmapSource(frame.ToBitmap());
                }
            }
            catch (Exception ex)
            {
                Log.Logger?.Information("Thrown exception in Video Capture: {Exception}", ex);
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