using System;
using System.IO;
using System.Windows;
using DryIoc;
using Microsoft.Extensions.Configuration;
using Serilog;
using FaceRecognizer.App.ViewModels;
using FaceRecognizer.App.ViewModels.Pages;
using FaceRecognizer.App.Views;
using FaceRecognizer.App.Views.Pages;

namespace FaceRecognizer.App
{
    /// <summary>
    /// Interaction logic for AppBase.xaml
    /// </summary>
    public partial class AppBase : Application
    {
        #region Static fields

        public static readonly IContainer Container = new Container();

        #endregion

        #region Lifecycle methods

        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = BuildConfiguration();
            Log.Logger = CreateLogger(configuration);
            Log.Logger?.Information("-------------------------------------------------");
            Log.Logger?.Information("Application started");

            RegisterTypes();

            Container.Resolve<MainWindow>().Show();
            base.OnStartup(e);
        }

        protected virtual void RegisterTypes()
        {
            Container.Register<MainWindow>(Reuse.Singleton);
            Container.Register<ImageRecognitionPage>(Reuse.Singleton);
            Container.Register<RealTimeRecognitionPage>(Reuse.Singleton);

            Container.Register<MainWindowViewModel>(Reuse.Singleton);
            Container.Register<ImageRecognitionPageViewModel>(Reuse.Singleton);
            Container.Register<RealTimeRecognitionPageViewModel>(Reuse.Singleton);
        }

        #endregion

        #region Static methods

        private static IConfiguration BuildConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .Build();
        }

        private static ILogger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        #endregion
    }
}
