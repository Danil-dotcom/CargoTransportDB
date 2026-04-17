using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CargoTransportation.Helpers
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return GetDefaultImage();

            string imageName = value.ToString();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string imagesPath = Path.Combine(baseDirectory, "Images", imageName);

            if (File.Exists(imagesPath))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagesPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return GetDefaultImage();
                }
            }

            return GetDefaultImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage GetDefaultImage()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string defaultPath = Path.Combine(baseDirectory, "Images", "no_image.png");

            if (File.Exists(defaultPath))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(defaultPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}