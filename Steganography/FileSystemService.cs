using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    public class FileSystemService
    {
        public long CalculateFileSize(string path)
        {
            ValidatePathNotNull(path);
            FileInfo fileinfo = new FileInfo(path);
            ValidateFileExists(fileinfo);
            return fileinfo.Length;
        }

        private void ValidateFileExists(FileInfo fileinfo)
        {
            if(!fileinfo.Exists)
            {
                throw new ValidationException("No file exists on the given path");
            } 
        }

        private void ValidatePathNotNull(string path)
        {
            if (path == null)
            {
                throw new NullReferenceException("Path must not be null");
            }
        }
    }
}
