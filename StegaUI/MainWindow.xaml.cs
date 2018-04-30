using Microsoft.Win32;
using Steganography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StegaUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel vm;
        private SteganographyService stegaService;

        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel();
            this.DataContext = vm;
            stegaService = new SteganographyService();
        }

        private void SelectCover_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.bmp) | *.bmp";
            if (ofd.ShowDialog() == true)
            {
                vm.CoverImage = System.IO.Path.GetFileName(ofd.FileName);
                vm.CoverImagePath = ofd.FileName;
                vm.CoverImageSize = stegaService.CalculateFileSize(ofd.FileName);
                vm.FreeBytes = stegaService.CalculateFreeBytesForMessage(ofd.FileName);
            }

        }

        private void SelectMessage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files(*.txt) | *.txt";
            if(ofd.ShowDialog() == true)
            {
                vm.MessagePath = ofd.FileName;
                vm.MessageFile = System.IO.Path.GetFileName(ofd.FileName);
                vm.MessageSize = stegaService.CalculateFileSize(ofd.FileName);
            }
        }

        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            stegaService.EncodeMessage(new EncodeRequest() { CoverPath = vm.CoverImagePath, MessagePath = vm.MessagePath });
        }
    }
}
