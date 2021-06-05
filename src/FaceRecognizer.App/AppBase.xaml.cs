using System.Windows;
using DryIoc;
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
        public static readonly IContainer Container = new Container();

        protected override void OnStartup(StartupEventArgs e)
        {
            RegisterTypes();

            Container.Resolve<MainWindow>().Show();
            base.OnStartup(e);
        }

        protected virtual void RegisterTypes()
        {
            Container.Register<MainWindow>(Reuse.Singleton);
            Container.Register<ImageRecognitionPage>(Reuse.Singleton);

            Container.Register<MainWindowViewModel>(Reuse.Singleton);
            Container.Register<ImageRecognitionPageViewModel>(Reuse.Singleton);
        }
    }
}
