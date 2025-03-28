using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Timetracker.Converter
{
    public class SmartTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
                return ts.ToString(@"hh\:mm");
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value?.ToString()?.Trim() ?? "";

            if(int.TryParse(input, out int raw))
            {
                if (input.Length <= 2)
                    return new TimeSpan(raw, 0, 0);
                else if (input.Length == 3)
                    return new TimeSpan(raw / 100, raw % 100, 0);
                else if (input.Length == 4)
                    return new TimeSpan(raw / 100, raw % 100, 0);
            }

            if (TimeSpan.TryParse(input, out TimeSpan result))
                return result;

            return new TimeSpan(0, 0, 0); // Fallback
        }
    }
}
