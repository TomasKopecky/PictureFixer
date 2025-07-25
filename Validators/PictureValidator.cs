using System.Drawing;

namespace PictureFixer.Validators
{
    public class PictureValidator : IPictureValidator
    {
        public bool IsValid(string filePath)
        {
            try
            {
                using var img = Image.FromFile(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
