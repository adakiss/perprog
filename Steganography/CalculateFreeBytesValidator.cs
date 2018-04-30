using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class CalculateFreeBytesValidator : BaseValidator
    {
        public void Validate(string path)
        {
            ValidateNotNull(path, "Path must not be null");
            ValidateFileExists(path, "Cover image does not exist");
            ValidateExtension(path, "bmp", "Cover image must have bmp extension");
        }
    }
}
