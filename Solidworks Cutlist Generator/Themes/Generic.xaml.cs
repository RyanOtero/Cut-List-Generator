using System;
using System.Windows;
using System.Windows.Input;

namespace Solidworks_Cutlist_Generator.Themes {
    public partial class Generic : ResourceDictionary {

        public Generic() {
            InitializeComponent();
        }

        /**
        * Header Buttons Events
        **/
        private void minimizeBtn_Click(object sender, RoutedEventArgs e) {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void maximizeBtn_Click(object sender, RoutedEventArgs e) {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized) {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            } else {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }

        }

        private void quitBtn_Click(object sender, RoutedEventArgs e) {
            Window.GetWindow(((FrameworkElement)e.Source)).Close();
        }

        /** 
        * Window Events
        **/
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Left) {
                Window.GetWindow(((FrameworkElement)e.Source)).DragMove();
            }

        }

    }
}