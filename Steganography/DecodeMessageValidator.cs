using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class DecodeMessageValidator : BaseValidator
    {
        public void Validate(DecodeRequest request)
        {
            ValidateNotNull(request, "Decode request must not be null");
            ValidateFileExists(request.EncodedMessagePath, "Given stego-image does not exist");
            ValidateExtension(request.EncodedMessagePath, "bmp", "Stego-image must have bmp extension");
        }
    }
}
