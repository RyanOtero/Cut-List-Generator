using System;
using System.Windows;
using System.Windows.Data;

namespace SolidPrice.Utils {
    [ValueConversion(typeof(float), typeof(string))]
    public class FloatToStringConverter : IValueConverter {
        public FloatToStringConverter() {
        }

        // Convert from float to string
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            return System.Convert.ToSingle(value).ToString();
        }

        // Convert from string to float
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            float? result = null;

            try {
                result = System.Convert.ToSingle(value);
            } catch {
            }

            return result.HasValue ? (object)result.Value : 24;
        }
    }
}
