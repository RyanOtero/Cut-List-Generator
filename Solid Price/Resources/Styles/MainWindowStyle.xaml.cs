using System.Windows;
using System.Windows.Input;

namespace SolidPrice.Styles {
    public partial class MainWindowStyle : ResourceDictionary {
        public MainWindowStyle() {
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
            Window win = Window.GetWindow(((FrameworkElement)e.Source));
            dynamic d = win.DataContext;
            d.CloseWin(win);
        }

        /** 
        * Window Events
        **/
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            Window win = App.Current.MainWindow;

            if (e.ClickCount > 1) {
                if (win.WindowState == WindowState.Maximized)
                    win.WindowState = WindowState.Normal;
                else
                    win.WindowState = WindowState.Maximized;
            }

            if (e.ChangedButton == MouseButton.Left) {
                Window.GetWindow(((FrameworkElement)e.Source)).DragMove();
            }
        }
    }
}