using AForge.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Tesseract;
using System.Drawing.Imaging;
using Emgu.CV.CvEnum;

namespace OCR_Checker
{
    /// <summary>
    /// Cần cài đặt nudget: 
    /// - Tesseract: 4.1.1
    /// - EmguCV: 4.1.1.3497
    /// - AFore: 2.2.5
    /// Cách dùng: Gọi hàm GetFilterNumber
    /// </summary>
    public class OCRHelper
    {
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        public static Bitmap ThreshHoldBinary(Bitmap bmp, byte threshold = 190)
        {
            Bitmap bmp1 = null;

            try
            {
                // Chuyển đổi đối tượng Bitmap thành đối tượng Image<Bgr, byte> của Emgu CV
                Image<Bgr, byte> image = new Image<Bgr, byte>(bmp);

                // Chuyển đổi ảnh thành ảnh grayscale
                Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

                // Chuyển đổi ảnh thành ảnh nhị phân
                Image<Gray, byte> binaryImage = grayImage.ThresholdBinary(new Gray(threshold), new Gray(255));

                // Chuyển đổi ảnh nhị phân thành đối tượng Bitmap
                bmp1 = binaryImage.ToBitmap();
            }
            catch { }

            return bmp1 ?? bmp;
        }
        public static Bitmap FilterThreshold(Bitmap b, byte threshold, out ResizeBilinear filter)
        {
            filter = new ResizeBilinear(b.Width, b.Height);
            return ThreshHoldBinary(filter.Apply((Bitmap)b.Clone()), threshold);
        }
        /// <summary>
        /// Gọi hàm này và truyền vào tham số b(Bitmap) bắt buộc: hình ảnh cần check
        /// scale là độ phóng to của hình để có thể check dễ hơn với hình nhỏ
        /// Threshold: điều chỉnh độ đen trắng của hình 
        /// blur: điều chỉnh độ mờ của hình
        /// </summary>
        /// <param name="b"></param>
        /// <param name="scale"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static string GetFilterNumber(Bitmap b, double? scale = 1, byte? threshold = 0, double blur = 0)
        {
            if (blur != 0)
            {
                var result = BlurImage(b, blur);
                if (result != null)
                {
                    b = result;
                }
            }

            if (threshold != 0)
            {
                ResizeBilinear filter;

                var result = FilterThreshold(b, (byte)threshold, out filter);
                if (result != null)
                {
                    b = result;
                }
            }

            if (scale != 1)
            {
                b = ScaleImage(b, (double)scale);
            }

            var OCR = new TesseractEngine("tessdata", "eng", Tesseract.EngineMode.TesseractAndLstm);
            //string template = "0123456789";
            //string template = "QWERTYUIOPASDFGHJKLZXCVBNM0123456789";
            string template = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789.:/>- #\\()_";
            //string template = "qwertyuiopasdfghjklzxcvbnm0123456789";
            //string template = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞßàáạảãâãäåæçèéêëìíîïð0123456789ỀỆẾỄỂẤẦẨẪẬỐỒỔỖỘỚỚỞỠỢèéẹẻẽềệếễểấầẩẫậốồổỗộớớởỡợọòỏõóưứỪỰỬỮỬẤẦẨẪẬỐỒỔỖỘỚỚỞỠỢỴỶỸỴỲỆỨỔỠỞỚỨỜỢƠỪỦỨỬỮÝỸỲỴỶƯƠỘỚỐỞỠỢỔỒỖỖỜỬỪỦỨỮẠỢỌỘỖỊỈỆỄỀẾỂỆỄẮẰẲẴẶẺẼẸẾỀỆỄỂỊỊẺỈỊỈỌỎỌỌỎỌỌỎỤỤỤỤỤỤỤỰỮỰỮỰỮỰỮỰỊỈỊỈỊỈỊỈỈỈỈỊỈỊỈỈỊỈỈỈỈỈỈỈỊỈỈỊỈỊỈỊỈỊỈỊỊỈỊỈỊỈỊỊỈỊỊỊỊỈỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊỊ .://>-#()";
            OCR.SetVariable("tessedit_char_whitelist", template);

            string res = "";
            try
            {
                var pix = PixConverter.ToPix(b);
                var page = OCR.Process(pix);
                res = page.GetText();
            }
            catch (Exception ee)
            {
                res = ee.Message;
            }
            return res;
        }
        public static Bitmap BlurImage(Bitmap b, double sigmaX)
        {
            if (b == null)
                return null;
            try
            {
                Image<Gray, float> inputImage = new Image<Gray, float>(BitmapToImage((Bitmap)b.Clone()));

                Image<Gray, float> smoothedImage = new Image<Gray, float>(inputImage.Width, inputImage.Height);
                CvInvoke.GaussianBlur(inputImage, smoothedImage, new Size(inputImage.Width % 2 == 0 ? inputImage.Width + 1 : inputImage.Width, inputImage.Height % 2 == 0 ? inputImage.Height + 1 : inputImage.Height), sigmaX);
                return smoothedImage.Bitmap;
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        public static float[,,] BitmapToImage(Bitmap bitmap)
        {
            // Get the bitmap's pixel format
            PixelFormat format = bitmap.PixelFormat;

            // Lock the bitmap's bits in memory
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, format);

            // Calculate the number of bytes per pixel based on the pixel format
            int bytesPerPixel = Image.GetPixelFormatSize(format) / 8;

            // Create the float array to hold the pixel values
            float[,,] floatArray = new float[bitmap.Height, bitmap.Width, bytesPerPixel];

            // Get the stride (number of bytes per row) of the bitmap
            int stride = bitmapData.Stride;

            // Create a pointer to the start of the bitmap data
            IntPtr scan0 = bitmapData.Scan0;

            // Copy the bitmap data to the float array
            unsafe
            {
                byte* pointer = (byte*)scan0;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int b = 0; b < bytesPerPixel; b++)
                        {
                            floatArray[y, x, b] = pointer[y * stride + x * bytesPerPixel + b] / 255f;
                        }
                    }
                }
            }

            // Unlock the bitmap's bits
            bitmap.UnlockBits(bitmapData);

            return floatArray;
        }

        public static byte[,,] BitmapToImageByte(Bitmap bitmap)
        {
            // Get the bitmap's pixel format
            PixelFormat format = bitmap.PixelFormat;

            // Lock the bitmap's bits in memory
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, format);

            // Calculate the number of bytes per pixel based on the pixel format
            int bytesPerPixel = Image.GetPixelFormatSize(format) / 8;

            // Create the float array to hold the pixel values
            byte[,,] floatArray = new byte[bitmap.Height, bitmap.Width, bytesPerPixel];

            // Get the stride (number of bytes per row) of the bitmap
            int stride = bitmapData.Stride;

            // Create a pointer to the start of the bitmap data
            IntPtr scan0 = bitmapData.Scan0;

            // Copy the bitmap data to the float array
            unsafe
            {
                byte* pointer = (byte*)scan0;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int b = 0; b < bytesPerPixel; b++)
                        {
                            floatArray[y, x, b] = (byte)(pointer[y * stride + x * bytesPerPixel + b] / 255f);
                        }
                    }
                }
            }

            // Unlock the bitmap's bits
            bitmap.UnlockBits(bitmapData);

            return floatArray;
        }

        public static System.Drawing.Bitmap ScaleImage(System.Drawing.Image a, double zoomin)
        {
            if (a == null)
                return null;

            double scale = zoomin;
            try
            {
                var b = ResizeImage(a, (int)(a.Size.Width * scale), (int)(a.Size.Height * scale));
                return b;
            } 
            catch (Exception ee)
            {
                return null;
            }
        }
        public static Bitmap CropImage(Image img, Rectangle cropRect)
        {
            Bitmap src = img as Bitmap;
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }

        public static Bitmap CropImage(Bitmap img, Rectangle cropRect)
        {
            Bitmap src = img;
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);


            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
