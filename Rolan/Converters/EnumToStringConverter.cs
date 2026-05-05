
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rolan.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                return enumValue.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                return Enum.Parse(targetType, stringValue);
            }
            return null;
        }
    }
}
