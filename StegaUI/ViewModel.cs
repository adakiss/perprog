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
    }
}
