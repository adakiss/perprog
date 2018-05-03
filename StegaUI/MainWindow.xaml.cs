using Microsoft.Win32;
using Steganography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                try
                {
                    vm.CoverImageSize = stegaService.CalculateFileSize(ofd.FileName);
                    vm.FreeBytes = stegaService.CalculateFreeBytesForMessage(ofd.FileName);
                }
                catch (ValidationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
                try
                {
                    vm.MessageSize = stegaService.CalculateFileSize(ofd.FileName);
                }
                catch (ValidationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                stegaService.EncodeMessageSeq(new EncodeRequest() { CoverPath = vm.CoverImagePath, MessagePath = vm.MessagePath, ResultPath=vm.EncodeResultPath });
                stopwatch.Stop();
                Debug.WriteLine(stopwatch.Elapsed);

                stopwatch.Reset();
                stopwatch.Start();
                stegaService.EncodeMessage(new EncodeRequest() { CoverPath = vm.CoverImagePath, MessagePath = vm.MessagePath, ResultPath = vm.EncodeResultPath });
                stopwatch.Stop();
                Debug.WriteLine(stopwatch.Elapsed);

                MessageBox.Show("Encoding finished", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(ValidationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                stegaService.DecodeMessage(new DecodeRequest() { EncodedMessagePath = vm.StegoImagePath, ResultPath = vm.ResultPath });
                stopwatch.Stop();
                Debug.WriteLine(stopwatch.Elapsed);

                stopwatch.Restart();
                stegaService.DecodeMessageSeq(new DecodeRequest() { EncodedMessagePath = vm.StegoImagePath, ResultPath = vm.ResultPath });
                stopwatch.Stop();
                Debug.WriteLine(stopwatch.Elapsed);

                MessageBox.Show("Decoding finished", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectStego_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.bmp) | *.bmp";
            if(ofd.ShowDialog() == true)
            {
                vm.StegoImagePath = ofd.FileName;
                vm.StegoImage = System.IO.Path.GetFileName(ofd.FileName);
                try
                {
                    vm.StegoImageSize = stegaService.CalculateFileSize(ofd.FileName);
                }
                catch ( ValidationException ex )
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SelectResult_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files(*.txt) | *.txt";
            if (ofd.ShowDialog() == true)
            {
                vm.ResultPath = ofd.FileName;
            }
        }

        private void SelectEncodeResult_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.bmp) | *.bmp";
            if (ofd.ShowDialog() == true)
            {
                vm.EncodeResultPath = ofd.FileName;
            }
        }
    }
}
