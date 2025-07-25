using WindowsAPICodePack.Dialogs;

namespace PictureFixer.Services
{
    public class FolderPickerDialog : IFolderPickerDialog
    {
        public string? PickFolder()
        {
            using CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
