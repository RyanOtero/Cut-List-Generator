﻿using System;
using System.Windows;
using System.Windows.Data;

namespace SolidPrice.Utils {
    [ValueConversion(typeof(double), typeof(string))]
    public class DecimalToStringConverter : IValueConverter {

        public DecimalToStringConverter() {
        }

        // Convert from decimal to string
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            return System.Convert.ToDecimal(value);
        }

        // Convert from string to decimal
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;

            decimal? result = null;

            try {
                result = System.Convert.ToDecimal(value);
            } catch {
            }

            return result.HasValue ? (object)result.Value : DependencyProperty.UnsetValue;
        }
    }
}
