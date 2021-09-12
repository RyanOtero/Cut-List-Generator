using SolidPrice.Views;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SolidPrice.Styles {
    public partial class MainWindowStyle : ResourceDictionary {

        private static bool isMaxed;
        private static double oldHeight;
        private static double oldWidth;
        private static double oldTop;
        private static double oldLeft;
        private static double virtualLeft;

        public MainWindowStyle() {
            InitializeComponent();
        }

        public static void GetVirtualWindowSize(Window win) {
            Window virtualWindow = new Window();
            virtualWindow.Top = win.Top;
            virtualWindow.Left = win.Left;
            virtualWindow.Width = 1f;
            virtualWindow.Height = 1f;
            virtualWindow.Opacity = 0;
            virtualWindow.Background = Brushes.Transparent;
            virtualWindow.Show();
            virtualWindow.WindowState = WindowState.Maximized;
            virtualLeft = virtualWindow.Left < 0 ? -virtualWindow.Width + 16 : 0;
            win.MaxHeight = virtualWindow.Height - 16;
            win.MaxWidth = virtualWindow.Width - 16;
            virtualWindow.Close();
        }

        public static void MaxState() {
            Window win = App.Current.MainWindow;
            GetVirtualWindowSize(win);
            oldHeight = win.Height;
            oldWidth = win.Width;
            oldTop = win.Top;
            oldLeft = win.Left;
            win.Height = win.MaxHeight;
            win.Width = win.MaxWidth;
            win.Top = 0;
            win.Left = virtualLeft;
            isMaxed = true;
        }

        public void NormalState() {
            Window win = App.Current.MainWindow;
            win.Height = oldHeight;
            win.Width = oldWidth;
            win.Top = oldTop;
            win.Left = oldLeft;
            isMaxed = false;
        }

        /**
        * Header Buttons Events
        **/
        private void minimizeBtn_Click(object sender, RoutedEventArgs e) {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void maximizeBtn_Click(object sender, RoutedEventArgs e) {
            Window win = Window.GetWindow(((FrameworkElement)e.Source));
            if (isMaxed) {
                NormalState();
            } else {
                MaxState();
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
        private void Window_MouseMove(object sender, MouseEventArgs e) {
            MainWindow win = App.Current.MainWindow as MainWindow;
            if (e.LeftButton == MouseButtonState.Pressed && isMaxed) {
                NormalState();
                win.Top = 0;
            }
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow((FrameworkElement)e.Source).DragMove();
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            Window win = App.Current.MainWindow;
            if (e.ClickCount > 1) {
                if (isMaxed)
                    NormalState();
                else {
                    MaxState();

                }
            }
        }
    }
}