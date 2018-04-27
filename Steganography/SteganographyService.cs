using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    public class SteganographyService
    {
        public long CalculateMaxBytesForContent(string path)
        {
            ValidatePathNotNull(path);
            ValidateFileExists(path);
            Bitmap image = (Bitmap)Image.FromFile(path);
            int pixelCount = image.Width * image.Height;
            int freeBits = pixelCount * 3;
            return pixelCount / 255;
        }

        private void ValidatePathNotNull(string path)
        {
            if(path == null)
            {
                throw new NullReferenceException("Path must not be null");
            }
        }

        private void ValidateFileExists(string path)
        {
            FileInfo fileinfo = new FileInfo(path);
            if(!fileinfo.Exists)
            {
                throw new ValidationException("The given file does not exist");
            }
        }

        private void ValidateExtension(string path)
        {
            if(Path.GetExtension(path) != "bmp")
            {
                throw new ValidationException("The extension must be .bmp for cover image");
            }
        }
    }
}
