using System.Windows;
using DryIoc;
using FaceRecognizer.App.ViewModels;

namespace FaceRecognizer.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = AppBase.Container.Resolve<MainWindowViewModel>();
            InitializeComponent();
        }
    }
}
