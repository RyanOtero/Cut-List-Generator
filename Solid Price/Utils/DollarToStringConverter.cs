using System;
using System.Windows;
using System.Windows.Data;

namespace SolidPrice.Utils {
    [ValueConversion(typeof(double), typeof(string))]
    public class DollarToStringConverter : IValueConverter {

        public DollarToStringConverter() {
        }

        // Convert from double to string
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            double doubleValue = System.Convert.ToDouble(value);
            return String.Format("{0:c}", doubleValue);
        }

        // Convert from string to double
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
                    if (value == null)
                return null;
            string s = value.ToString();
            if (s.Length > 0) {
                if (s[0] == '$') {
                    s = s.Remove(0, 1);
                }
            }
            double? result = null;

            try {
                result = System.Convert.ToDouble(s);
            } catch {
            }

            return result.HasValue ? (object)result.Value : 0;
        }
    }
}
