
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rolan.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iconPath = value as string;
            
            if (string.IsNullOrEmpty(iconPath))
                return GetDefaultIcon();

            try
            {
                if (File.Exists(iconPath))
                {
                    return LoadIcon(iconPath);
                }
            }
            catch
            {
            }

            return GetDefaultIcon();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private ImageSource LoadIcon(string path)
        {
            try
            {
                string ext = Path.GetExtension(path).ToLower();
                
                if (ext == ".ico")
                {
                    return BitmapFrame.Create(new Uri(path), BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
                }
                else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                {
                    return BitmapFrame.Create(new Uri(path), BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
                }
                else
                {
                    return ExtractIconFromExe(path);
                }
            }
            catch
            {
                return GetDefaultIcon();
            }
        }

        private ImageSource ExtractIconFromExe(string path)
        {
            try
            {
                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(path))
                {
                    if (icon != null)
                    {
                        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }
            }
            catch
            {
            }
            return GetDefaultIcon();
        }

        private ImageSource GetDefaultIcon()
        {
            try
            {
                using (var icon = System.Drawing.SystemIcons.Application)
                {
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
