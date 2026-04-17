using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CargoTransportation.Helpers
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == "Available")
                return new SolidColorBrush(Colors.Green);
            else if (status == "OnRoute")
                return new SolidColorBrush(Colors.Orange);
            else if (status == "Repair")
                return new SolidColorBrush(Colors.Red);
            else
                return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}