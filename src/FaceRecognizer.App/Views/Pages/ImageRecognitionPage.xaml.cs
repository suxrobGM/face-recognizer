using System.Windows.Controls;
using DryIoc;
using FaceRecognizer.App.ViewModels.Pages;

namespace FaceRecognizer.App.Views.Pages
{
    /// <summary>
    /// Interaction logic for ImageRecognitionPage.xaml
    /// </summary>
    public partial class ImageRecognitionPage : Page
    {
        public ImageRecognitionPage()
        {
            DataContext = AppBase.Container.Resolve<ImageRecognitionPageViewModel>();
            InitializeComponent();
        }
    }
}
