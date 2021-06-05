using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceRecognizer.App.Core.Helpers
{
    /// <summary>
    /// Image helpers
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Converts image bytes to <see cref="BitmapImage"/>
        /// </summary>
        /// <param name="imageBytes">The source image bytes.</param>
        /// <returns></returns>
        public static BitmapImage ConvertToBitmapImage(byte[] imageBytes)
        {
            using var memory = new MemoryStream(imageBytes)
            {
                Position = 0
            };

            var bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.StreamSource = memory;
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.EndInit();

            return bmpImage;
        }

        /// <summary>
        /// Converts a <see cref="Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="bitmap">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            BitmapSource bitSrc;
            var hBitmap = bitmap.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }
    }

    internal static class NativeMethods
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);
    }
}