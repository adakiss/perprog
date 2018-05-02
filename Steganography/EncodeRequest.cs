using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    public class EncodeRequest
    {
        public string CoverPath { get; set; }
        public string MessagePath { get; set; }
        public string ResultPath { get; set; }
    }
}
