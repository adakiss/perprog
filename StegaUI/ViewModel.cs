using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegaUI
{
    class ViewModel : NotifyProperyChanged
    {
        private string coverImage;
        private long coverImageSize;
        private long freeBytes;
        private string messageFile;
        private long messageSize;
        private string stegoImage;
        private string resultPath;
        private long stegoImageSize;
        private string encodeResultPath;

        public string CoverImage
        {
            get { return coverImage; }
            set
            {
                this.coverImage = value;
                NotifyPropertyChanged();
            }
        }

        public String CoverImagePath { get; set; }
        public long CoverImageSize
        {
            get { return coverImageSize; }
            set
            {
                this.coverImageSize = value;
                NotifyPropertyChanged();
            }
        }

        public long FreeBytes
        {
            get { return freeBytes; }
            set
            {
                this.freeBytes = value;
                NotifyPropertyChanged();
            }
        }

        public string MessagePath { get; set; }

        public string MessageFile
        {
            get { return messageFile; }
            set
            {
                this.messageFile = value;
                NotifyPropertyChanged();
            }
        }

        public long MessageSize
        {
            get { return messageSize; }
            set
            {
                this.messageSize = value;
                NotifyPropertyChanged();
            }
        }

        public string StegoImage
        {
            get { return stegoImage; }
            set
            {
                stegoImage = value;
                NotifyPropertyChanged();
            }
        }

        public string StegoImagePath { get; set; }

        public string ResultPath
        {
            get { return resultPath; }
            set
            {
                resultPath = value;
                NotifyPropertyChanged();
            }
        }

        public long StegoImageSize
        {
            get { return stegoImageSize; }
            set
            {
                stegoImageSize = value;
                NotifyPropertyChanged();
            }
        }

        public string EncodeResultPath
        {
            get { return encodeResultPath; }
            set
            {
                encodeResultPath = value;
                NotifyPropertyChanged();
            }
        }
    }
}
