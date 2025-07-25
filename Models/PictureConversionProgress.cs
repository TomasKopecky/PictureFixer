namespace PictureFixer.Modals
{
    public class PictureConversionProgress
    {
        public string File { get; set; } = "";
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
