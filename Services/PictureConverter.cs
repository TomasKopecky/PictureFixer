using PictureFixer.Modals;
using PictureFixer.Validators;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PictureFixer.Services
{
    public class PictureConverter : IPictureConverter
    {
        private const string OUTPUT_EXTENSION = ".jpg";

        public void Convert(
            IEnumerable<string> files,
            string destinationFolder,
            IProgress<PictureConversionProgress> progress,
            CancellationToken cancellationToken = default)
        {
            foreach (string file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (File.Exists(file))
                {
                    // Generate destination filename with the chosen extension
                    string destinationFileName = Path.Combine(
                        destinationFolder,
                        Path.GetFileNameWithoutExtension(file) + OUTPUT_EXTENSION
                    );

                    try
                    {
                        using var image = Image.FromFile(file); // Load image
                        image.Save(destinationFileName, ImageFormat.Jpeg); // Save as JPG

                        progress.Report(new PictureConversionProgress
                        {
                            File = file,
                            IsSuccess = true
                        });
                    }
                    catch (Exception ex)
                    {
                        progress.Report(new PictureConversionProgress
                        {
                            File = file,
                            IsSuccess = false,
                            ErrorMessage = ex.Message
                        });
                    }
                }
            }
        }
    }
}
