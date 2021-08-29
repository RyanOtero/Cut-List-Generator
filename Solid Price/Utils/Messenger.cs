using Solid_Price.Models;
using Solid_Price.Resources.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBoxImage = Solid_Price.Models.MessageBoxImage;

namespace Solid_Price.Utils {
    public static class Messenger {
        public static void ErrorMessage(string caption, string text) {
            Application.Current.Dispatcher.Invoke(delegate {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageWindow.Show(caption, text, button, icon);
            });
            return;
        }
        public static MessageBoxResult YesNoMessage(string caption, string text) {
            MessageBoxResult msgResult = MessageBoxResult.None;
            msgResult = Application.Current.Dispatcher.Invoke(delegate {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                return MessageWindow.Show(caption, text, button, icon);
            });
            return msgResult;
        }
    }
}
