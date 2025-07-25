using System.Windows;

namespace PictureFixer.Services
{
    internal class MessageBoxService : IMessageBoxService
    {
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
