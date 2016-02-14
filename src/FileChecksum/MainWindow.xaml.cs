using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace CompliaShield.FileChecksum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.compliashield.com/apps/filechecksum");
        }

        private void file_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            var fileName = files[0];
            this.LoadFile(fileName);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                this.LoadFile(openFileDialog.FileName);
            }
        }

        void LoadFile(string fileName)
        {
            string fileNameShort = null;
            string sha1Value = null;
            string md5Value = null;

            try
            {
                var fi = new FileInfo(fileName);
                fileNameShort = fi.Name;
                using (var sha1 = SHA1.Create())
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        sha1Value = BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        md5Value = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
            }
            catch (Exception)
            {
                ShowErrorMessage("File could not be processed.");
                return;
            }

            tboxfileName.Text = fileNameShort;
            tboxMd5.Text = md5Value;
            tboxSha1.Text = sha1Value;

        }


        private void ShowErrorMessage(string message)
        {
            lblError.Content = message;
            lblError.Visibility = Visibility.Visible;
            tboxfileName.Text = null;
            tboxMd5.Text = null;
            tboxSha1.Text = null;

            // Create a Timer with a Normal Priority
            _timer = new DispatcherTimer();

            // Set the Interval to 2 seconds
            _timer.Interval = TimeSpan.FromMilliseconds(5000);

            // Set the callback to just show the time ticking away
            // NOTE: We are using a control so this has to run on 
            // the UI thread
            _timer.Tick += new EventHandler(delegate (object s, EventArgs a)
            {
                lblError.Content = null;
                lblError.Visibility = Visibility.Hidden;
                _timer.Stop();
            });

            // Start the timer
            _timer.Start();
        }

        private void txtGuide_Copy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var winLicense = new LicenseWindow();
            winLicense.Show();
        }
    }
}
