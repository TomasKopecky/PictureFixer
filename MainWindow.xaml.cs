using PictureFixer.ViewModels;
using Wpf.Ui.Controls;

namespace PictureFixer
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        private readonly PictureTransformViewModel _viewModel;
        public MainWindow(PictureTransformViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
