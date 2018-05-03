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
        private DecodeMessageValidator decodeValidator;

        public SteganographyService()
        {
            freeByteCalcValidator = new CalculateFreeBytesValidator();
            fileSizeCalcValidator = new FileSizeCalcValidator();
            encodeValidator = new EncodeMessageValidator();
            decodeValidator = new DecodeMessageValidator();
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
            int headerBits = new BitArray(BitConverter.GetBytes(freeBits)).Count;
            return (freeBits - headerBits) / 8;
        }

        public void EncodeMessage(EncodeRequest request)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            encodeValidator.Validate(request);
            Bitmap coverImage = new Bitmap(request.CoverPath);
            Rectangle rect = new Rectangle(0, 0, coverImage.Width, coverImage.Height);
            BitmapData coverData = coverImage.LockBits(rect, ImageLockMode.ReadWrite, coverImage.PixelFormat);
            IntPtr ptr = coverData.Scan0;
            int bytes = Math.Abs(coverData.Stride) * coverImage.Height;
            BitArray messageWithHeader = CreateMessageWithHeader(bytes, request.MessagePath);
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
            EncodeMessageIntoCover(messageWithHeader, bytes, ptr);
            coverImage.UnlockBits(coverData);
            coverImage.Save(request.ResultPath);
        }

        public void EncodeMessageSeq(EncodeRequest request)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            encodeValidator.Validate(request);
            Bitmap coverImage = new Bitmap(request.CoverPath);
            Rectangle rect = new Rectangle(0, 0, coverImage.Width, coverImage.Height);
            BitmapData coverData = coverImage.LockBits(rect, ImageLockMode.ReadWrite, coverImage.PixelFormat);
            IntPtr ptr = coverData.Scan0;
            int bytes = Math.Abs(coverData.Stride) * coverImage.Height;
            BitArray messageWithHeader = CreateMessageWithHeader(bytes, request.MessagePath);
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
            EncodeMessageIntoCoverSeq(messageWithHeader, bytes, ptr);
            coverImage.UnlockBits(coverData);
            coverImage.Save(request.ResultPath);
        }

        private BitArray CreateHeader(int messageLength, int maxBytes)
        {
            BitArray header = new BitArray(BitConverter.GetBytes(maxBytes));
            BitArray messageLengthBinary = new BitArray(BitConverter.GetBytes(messageLength));
            header.SetAll(false);
            for (int i = 0; i < messageLengthBinary.Count; i++)
            {
                if (messageLengthBinary[i])
                {
                    header[i] = true;
                }
            }
            return header;
        }

        private BitArray AddHeaderToMessage(BitArray header, BitArray messageContent)
        {
            BitArray messageWithHeader = new BitArray(header.Count + messageContent.Count);
            for (int i = 0; i < header.Count; i++)
            {
                messageWithHeader[i] = header[i];
            }
            for (int j = 0; j < messageContent.Count; j++)
            {
                messageWithHeader[j + header.Count] = messageContent[j];
            }
            return messageWithHeader;
        }

        private BitArray CreateMessageWithHeader(int allBytes, string messageContentPath)
        {
            BitArray message = new BitArray(File.ReadAllBytes(messageContentPath));
            BitArray header = CreateHeader(message.Count, allBytes);
            return AddHeaderToMessage(header, message);
        }

        private void EncodeMessageIntoCover(BitArray message, int allBytes, IntPtr firstByte)
        {
            byte[] rgbValues = new byte[allBytes];
            bool[] arr = new bool[message.Count];
            message.CopyTo(arr, 0);
            Marshal.Copy(firstByte, rgbValues, 0, allBytes);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Parallel.For(0, arr.Length,
                index => {
                    if (arr[index])
                    {
                        rgbValues[index] -= (byte)(rgbValues[index] % 2);
                        rgbValues[index]++;
                    }
                    else
                    {
                        rgbValues[index] -= (byte)(rgbValues[index] % 2);
                    }
                }
            );
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
            Marshal.Copy(rgbValues, 0, firstByte, allBytes);
        }

        private void EncodeMessageIntoCoverSeq(BitArray message, int allBytes, IntPtr firstByte)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            byte[] rgbValues = new byte[allBytes];
            bool[] arr = new bool[message.Count];
            message.CopyTo(arr, 0);
            Marshal.Copy(firstByte, rgbValues, 0, allBytes);
            for(int i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                {
                    rgbValues[i] -= (byte)(rgbValues[i] % 2);
                    rgbValues[i]++;
                }
                else
                {
                    rgbValues[i] -= (byte)(rgbValues[i] % 2);
                }
            }
            Marshal.Copy(rgbValues, 0, firstByte, allBytes);
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
        }

        public void DecodeMessage(DecodeRequest request)
        {
            decodeValidator.Validate(request);
            Bitmap encodedMessage = new Bitmap(request.EncodedMessagePath);
            Rectangle rect = new Rectangle(0, 0, encodedMessage.Width, encodedMessage.Height);
            BitmapData encodedData = encodedMessage.LockBits(rect, ImageLockMode.ReadWrite, encodedMessage.PixelFormat);
            IntPtr ptr = encodedData.Scan0;
            int bytes = Math.Abs(encodedData.Stride) * encodedMessage.Height;
            BitArray lsbitArray = ReadAllLSBits(bytes, ptr);
            encodedMessage.UnlockBits(encodedData);
            BitArray header = GetHeader(lsbitArray, bytes);
            List<byte> decodedBytes = DecodeBytes(lsbitArray, header);
            File.WriteAllBytes(request.ResultPath, decodedBytes.ToArray());
        }

        public void DecodeMessageSeq(DecodeRequest request)
        {
            decodeValidator.Validate(request);
            Bitmap encodedMessage = new Bitmap(request.EncodedMessagePath);
            Rectangle rect = new Rectangle(0, 0, encodedMessage.Width, encodedMessage.Height);
            BitmapData encodedData = encodedMessage.LockBits(rect, ImageLockMode.ReadWrite, encodedMessage.PixelFormat);
            IntPtr ptr = encodedData.Scan0;
            int bytes = Math.Abs(encodedData.Stride) * encodedMessage.Height;
            BitArray lsbitArray = ReadAllLSBitsSeq(bytes, ptr);
            encodedMessage.UnlockBits(encodedData);
            BitArray header = GetHeader(lsbitArray, bytes);
            List<byte> decodedBytes = DecodeBytes(lsbitArray, header);
            File.WriteAllBytes(request.ResultPath, decodedBytes.ToArray());
        }

        private BitArray ReadAllLSBits(int allBytes, IntPtr firstByte)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitArray lsbitArray = new BitArray(allBytes);
            bool[] arr = new bool[lsbitArray.Count];
            lsbitArray.CopyTo(arr, 0);
            byte[] rgbValues = new byte[allBytes];
            Marshal.Copy(firstByte, rgbValues, 0, allBytes);
            Parallel.For(0, allBytes,
                index => {
                    arr[index] = (rgbValues[index] % 2) == 1;
                }
            );
            Marshal.Copy(rgbValues, 0, firstByte, allBytes);
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
            return lsbitArray;
        }

        private BitArray ReadAllLSBitsSeq(int allBytes, IntPtr firstByte)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitArray lsbitArray = new BitArray(allBytes);
            byte[] rgbValues = new byte[allBytes];
            Marshal.Copy(firstByte, rgbValues, 0, allBytes);
            for(int i = 0; i < allBytes; i++)
            {
                lsbitArray[i] = (rgbValues[i] % 2) == 1;
            }
            Marshal.Copy(rgbValues, 0, firstByte, allBytes);
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
            return lsbitArray;
        }

        private BitArray GetHeader(BitArray message, int allBytes)
        {
            BitArray header = new BitArray(BitConverter.GetBytes(allBytes));
            for (int i = 0; i < header.Length; i++)
            {
                header[i] = message[i];
            }
            return header;
        }

        private List<byte> DecodeBytes(BitArray content, BitArray header)
        {
            int messageLength = ConvertUtils.GetIntFromBitArray(header);
            int currentByteIndex = 0;
            List<byte> byteList = new List<byte>();
            BitArray currentByte = new BitArray(8);
            for (int j = header.Count; j < header.Count + messageLength; j++)
            {
                currentByte[currentByteIndex] = content[j];
                currentByteIndex++;
                if (currentByteIndex == 8)
                {
                    byteList.Add(ConvertUtils.GetByteFromBitArray(currentByte));
                    currentByteIndex = 0;
                }
            }
            return byteList;
        }
    }
}
