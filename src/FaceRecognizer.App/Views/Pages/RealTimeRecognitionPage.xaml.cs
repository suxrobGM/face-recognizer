using System.Windows.Controls;
using DryIoc;
using FaceRecognizer.App.ViewModels.Pages;

namespace FaceRecognizer.App.Views.Pages
{
    /// <summary>
    /// Interaction logic for RealTimeRecognitionPage.xaml
    /// </summary>
    public partial class RealTimeRecognitionPage : Page
    {
        public RealTimeRecognitionPage()
        {
            DataContext = AppBase.Container.Resolve<RealTimeRecognitionPageViewModel>();
            InitializeComponent();
        }
    }
}
