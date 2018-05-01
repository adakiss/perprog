using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine(messageLength.ToString());
            Debug.WriteLine(BitConverter.GetBytes(messageLength));
            BitArray messageLengthBinary = new BitArray(BitConverter.GetBytes(messageLength));
            logBitArray(messageLengthBinary);
            BitArray header = new BitArray(BitConverter.GetBytes(bytes));
            header.SetAll(false);

           // int lastSetHeaderIndex = header.Length;
            for(int i = 0; i < messageLengthBinary.Count; i++)
            {
                if(messageLengthBinary[i])
                {
                    header[i] = true;
                }
            }
            logBitArray(header);


            BitArray messageWithHeader = new BitArray(header.Count + message.Count);
            for(int j = 0; j < header.Count; j++)
            {
                messageWithHeader[j] = header[j];
            }
            for(int k = 0; k < message.Count; k++)
            {
                messageWithHeader[k + header.Count] = message[k];
            }

            logBitArray(messageWithHeader);
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

        private void logBitArray(BitArray arr)
        {
            String str = "";
            for (int x = 0; x < arr.Count; x++)
            {
                str += arr[x] ? "1" : "0";
            }
            Debug.WriteLine(str);
        }

        public void DecodeMessage(DecodeRequest request)
        {
            //TODO validate

            Bitmap encodedMessage = new Bitmap(request.EncodedMessagePath);
            Rectangle rect = new Rectangle(0, 0, encodedMessage.Width, encodedMessage.Height);
            BitmapData encodedData = encodedMessage.LockBits(rect, ImageLockMode.ReadWrite, encodedMessage.PixelFormat);
            IntPtr ptr = encodedData.Scan0;
            int bytes = Math.Abs(encodedData.Stride) * encodedMessage.Height;

            BitArray lsbitArray = new BitArray(bytes);
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            Parallel.For(0, bytes,
                index => {
                        lsbitArray[index] = (rgbValues[index] % 2) == 1;
                    }
            );
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            encodedMessage.UnlockBits(encodedData);

            BitArray header = new BitArray(BitConverter.GetBytes(bytes));
            for(int i = 0; i < header.Length; i++)
            {
                header[i] = lsbitArray[i];
            }
            logBitArray(header);
            //logBitArray(lsbitArray);

            int messageLength = getIntFromBitArray(header);
            int currentByteIndex = 0;
            List<byte> byteList = new List<byte>();
            BitArray currentByte = new BitArray(8);
            for (int j = header.Count; j < header.Count + messageLength; j++)
            {
                currentByte[currentByteIndex] = lsbitArray[j];
                currentByteIndex++;
                if(currentByteIndex == 8)
                {
                    byteList.Add(getByteFromBitArray(currentByte));
                    currentByteIndex = 0;
                }
            }
            File.WriteAllBytes(request.ResultPath, byteList.ToArray());
            Debug.WriteLine(messageLength.ToString());
        }

        private int getIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }

        private byte getByteFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 8)
                throw new ArgumentException("Argument length shall be at most 8 bits.");

            byte[] array = new byte[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }
    }
}
