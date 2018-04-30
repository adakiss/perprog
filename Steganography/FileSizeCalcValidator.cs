using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class FileSizeCalcValidator : BaseValidator
    {
        public void Validate(string path)
        {
            ValidateNotNull(path, "Path must not be null");
            ValidateFileExists(path, "Given file does not exist");
        }
    }
}
