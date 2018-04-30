using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    public class SteganographyService
    {
        private CalculateFreeBytesValidator freeByteCalcValidator;
        private EncodeMessageValidator encodeValidator;
        private FileSizeCalcValidator fileSizeCalcValidator;

        public SteganographyService()
        {
            freeByteCalcValidator = new CalculateFreeBytesValidator();
            fileSizeCalcValidator = new FileSizeCalcValidator();
            encodeValidator = new EncodeMessageValidator();
        }

        public long CalculateFileSize(string path)
        {
            fileSizeCalcValidator.Validate(path);
            FileInfo fileinfo = new FileInfo(path);
            return fileinfo.Length;
        }

        public long CalculateFreeBytesForMessage(string path)
        {
            freeByteCalcValidator.Validate(path);
            Bitmap image = (Bitmap)Image.FromFile(path);
            int pixelCount = image.Width * image.Height;
            int freeBits = pixelCount * 3;
            long maxFreeBytes = pixelCount / 8;
            long headerBytes = maxFreeBytes / 255;
            return maxFreeBytes - headerBytes;
        }

        public void EncodeMessage(EncodeRequest request)
        {
            encodeValidator.Validate(request);

            Bitmap coverImage = new Bitmap(request.CoverPath);
            Rectangle rect = new Rectangle(0, 0, coverImage.Width, coverImage.Height);
            BitmapData coverData = coverImage.LockBits(rect, ImageLockMode.ReadWrite, coverImage.PixelFormat);
            IntPtr ptr = coverData.Scan0;
            int bytes = Math.Abs(coverData.Stride) * coverImage.Height;


            BitArray message = new BitArray(File.ReadAllBytes(request.MessagePath));
            int messageLength = message.Count;
            BitArray messageLengthBinary = new BitArray(BitConverter.GetBytes(messageLength));
            BitArray mlinb = new BitArray(bytes);
            for(int i = 0; i < messageLengthBinary.Count; i++)
            {
                if(messageLengthBinary[i])
                {
                    mlinb[i] = true;
                }
            }

            BitArray messageWithHeader = new BitArray(mlinb.Count + message.Count);
            for(int j = 0; j < mlinb.Count; j++)
            {
                messageWithHeader[j] = mlinb[j];
            }
            for(int k = 0; k < message.Count; k++)
            {
                messageWithHeader[k + mlinb.Count] = message[k];
            }

            
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            Parallel.For(0, bytes, 
                index => {
                    if(index < messageWithHeader.Count)
                    {
                        if(messageWithHeader[index])
                        {
                            rgbValues[index] -= (byte)(rgbValues[index] % 2);
                            rgbValues[index]++;
                        } else
                        {
                            rgbValues[index] -= (byte)(rgbValues[index] % 2);
                        }
                    }
                }
            );
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            coverImage.UnlockBits(coverData);
            coverImage.Save("result.bmp");
        }
    }
}
