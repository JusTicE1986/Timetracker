using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Timetracker.Converter
{
    class TimeSpanToReadableTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is TimeSpan ts)
            {
                int gesamtMinuten = (int)ts.TotalMinutes;
                int stunden = gesamtMinuten / 60;
                int minuten = gesamtMinuten % 60;
                return $"{stunden}:{minuten:D2} h";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
