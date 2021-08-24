using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Solid_Price.Utils {
    public static class Messenger {
        public static void ErrorMessage(string caption, string text) {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(text, caption, button, icon, MessageBoxResult.Yes);
        }
        public static MessageBoxResult YesNoMessage(string caption, string text) {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            return MessageBox.Show(text, caption, button, icon, MessageBoxResult.Yes);
        }
    }
}
