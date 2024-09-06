using AForge.Imaging.Filters;
using Microsoft.Win32;
using OCR_Checker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

namespace OCR_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage _imageSource;
        private string _imagePath;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadImages();
        }

        void LoadImages()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                _imagePath = dialog.FileName;
                imgMain.Source = new BitmapImage(new Uri(_imagePath));
            }
        }

        void CheckOCR()
        {
            txbResult.Text = OCR_Checker.OCRHelper.GetFilterNumber(new System.Drawing.Bitmap(_imagePath), Int32.Parse(txbScale.Text), byte.Parse(txbThreshold.Text));
        }

        //void CheckOCR()
        //{
        //    try
        //    {
        //        // Update the OCR result in the result box
        //        txbResult.Text = OCR_Checker.OCRHelper.GetFilterNumber(new System.Drawing.Bitmap(imagePath), Int32.Parse(txbScale.Text), byte.Parse(txbThreshold.Text));

        //        // Refresh the image to reflect changes in real-time
        //        checkImg.Source = new BitmapImage(new Uri(imagePath));
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle potential errors in parsing or loading
        //        txbResult.Text = "Error: " + ex.Message;
        //    }
        //}

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                // Save the bitmap to the memory stream in PNG format
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0; // Reset the position of the stream

                // Create a BitmapImage and load it from the memory stream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        //private void UpdateImageInRealTime()
        //{
        //    if (string.IsNullOrEmpty(_imagePath))
        //        return;

        //    // Load the original image
        //    Bitmap originalBitmap = new Bitmap(_imagePath);

        //    // Apply threshold and scaling
        //    Bitmap processedBitmap = OCR_Checker.OCRHelper.FilterThreshold(originalBitmap,
        //        byte.Parse(txbThreshold.Text), out ResizeBilinear filter);

        //    // If scaling is applied
        //    if (Int32.Parse(txbScale.Text) != 1)
        //    {
        //        processedBitmap = OCR_Checker.OCRHelper.ScaleImage(processedBitmap, Int32.Parse(txbScale.Text));
        //    }

        //    // Update the Image control with the processed image
        //    checkImg.Source = BitmapToBitmapImage(processedBitmap);
        //}

        private void UpdateImageInRealTime()
        {
            if (_imagePath == null) { return; }
            Bitmap originalImage = new Bitmap(_imagePath);

            Bitmap changedBitmap = OCR_Checker.OCRHelper.FilterThreshold(originalImage, byte.Parse(txbThreshold.Text), out ResizeBilinear filter);

            if (Int32.Parse(txbScale.Text) != 0)
            {
                changedBitmap = OCR_Checker.OCRHelper.ScaleImage(changedBitmap, Int32.Parse(txbThreshold.Text));
            }

            checkImg.Source = BitmapToBitmapImage(changedBitmap);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CheckOCR();
        }

        private void SliderScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txbScale != null)
            {
                txbScale.Text = ((int)sliderScale.Value).ToString();
                if (imgMain.Source != null)
                {
                    UpdateImageInRealTime();
                }
            }
        }

        // Event handler for Threshold slider
        private void SliderThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txbThreshold != null)
            {
                txbThreshold.Text = ((int)sliderThreshold.Value).ToString();
                if (imgMain.Source != null)
                {
                    UpdateImageInRealTime();
                }
            }
        }
    }
}