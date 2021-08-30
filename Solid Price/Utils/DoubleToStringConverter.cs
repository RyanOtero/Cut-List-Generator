using System;
using System.Windows;
using System.Windows.Data;

namespace SolidPrice.Utils {
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : IValueConverter {

        public DoubleToStringConverter() {
        }

        // Convert from double to string
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            double doubleValue = System.Convert.ToDouble(value);
            return String.Format("{0:F3}", doubleValue);
        }

        // Convert from string to double
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            double? result = null;

            try {
                result = System.Convert.ToDouble(value);
            } catch {
            }

            return result.HasValue ? (object)result.Value : DependencyProperty.UnsetValue;
        }
    }
}
