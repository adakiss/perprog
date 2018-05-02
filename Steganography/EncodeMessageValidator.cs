using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class EncodeMessageValidator : BaseValidator
    {
        public void Validate(EncodeRequest request)
        {
            ValidateNotNull(request, "Request must not be null");
            ValidateCoverImage(request.CoverPath);
            ValidateMessage(request.MessagePath);
            ValidateMessageSize(request);
            ValidateResultPath(request.ResultPath);
        }

        private void ValidateCoverImage(string coverPath)
        {
            if(coverPath == null)
            {
                throw new ValidationException("Cover image has not been selected");
            }
            ValidateFileExists(coverPath, "Cover image does not exist");
            ValidateExtension(coverPath, "bmp", "Cover image must have bmp extension");
        }

        private void ValidateMessage(string messagePath)
        {
            if(messagePath == null)
            {
                throw new ValidationException("Message file has not been selected");
            }
            ValidateFileExists(messagePath, "Message file does not exist");
            ValidateExtension(messagePath, "txt", "Message file must have txt extension");
        }

        private void ValidateMessageSize(EncodeRequest request)
        {
            FileInfo messageInfo = new FileInfo(request.MessagePath);
            Bitmap image = (Bitmap)Image.FromFile(request.CoverPath);
            int pixelCount = image.Width * image.Height;
            int freeBits = pixelCount * 3;
            int headerBits = new BitArray(BitConverter.GetBytes(freeBits)).Count;
            if (freeBits - headerBits < messageInfo.Length)
            {
                throw new ValidationException("Message size is greater the the free bytes in cover image");
            }
        }

        private void ValidateResultPath(string resultPath)
        {
            if(resultPath == null)
            {
                throw new ValidationException("Result file has not been selected");
            }
            ValidateFileExists(resultPath, String.Format("Result file [{0}] does not exist", resultPath));
            ValidateExtension(resultPath, "bmp", "Result file must have bmp extension");
        }
    }
}
