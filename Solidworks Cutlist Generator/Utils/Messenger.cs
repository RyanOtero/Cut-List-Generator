using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Solidworks_Cutlist_Generator.Utils {
    public static class Messenger {
        public static void ErrorMessage(string caption, string text) {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;
            result = MessageBox.Show(text, caption, button, icon, MessageBoxResult.Yes);
        }
    }
}
