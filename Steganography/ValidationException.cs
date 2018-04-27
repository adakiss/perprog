using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class ValidationException : Exception
    {
        public ValidationException(string msg) : base(msg) { }
    }
}
