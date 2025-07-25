using PictureFixer.Modals;

namespace PictureFixer.Services
{
    public interface IPictureConverter
    {
        void Convert(
            IEnumerable<string> files,
            string destinationFolder,
            IProgress<PictureConversionProgress> progress,
            CancellationToken cancellationToken = default);
    }
}
