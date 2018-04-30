using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class BaseValidator
    {
        protected void ValidateNotNull(object param, string message)
        {
            if(param == null)
            {
                throw new NullReferenceException(message);
            }
        }

        protected void ValidateExtension(string path, string extension, string message)
        {
            if (Path.GetExtension(path) != "." + extension)
            {
                throw new ValidationException(message);
            }
        }

        protected void ValidateFileExists(string path, string message)
        {
            FileInfo fileinfo = new FileInfo(path);
            if (!fileinfo.Exists)
            {
                throw new ValidationException("The given file does not exist");
            }
        }
    }
}
