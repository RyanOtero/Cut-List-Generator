using SolidPrice.Views;
using System.Windows;
using System.Windows.Input;

namespace SolidPrice.Styles {
    public partial class MainWindowStyle : ResourceDictionary {

        private static bool isMaxed;
        private static double oldHeight;
        private static double oldWidth;
        private static double oldTop;
        private static double oldLeft;


        public MainWindowStyle() {
            InitializeComponent();
        }

        public static void PostitionWindowOnScreen(Window window, double horizontalShift = 0, double verticalShift = 0) {
            WpfScreenHelper.Screen screen = WpfScreenHelper.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
            window.Width = screen.Bounds.Width;
            window.Height = screen.Bounds.Height - 2 * -verticalShift;
            window.Left = screen.Bounds.X + ((screen.Bounds.Width - window.ActualWidth) / 2) + horizontalShift;
            window.Top = screen.Bounds.Y + ((screen.Bounds.Height - window.ActualHeight) / 2) + verticalShift;
        }


        public static void MaxState() {
            Window win = App.Current.MainWindow;
            oldHeight = win.Height;
            oldWidth = win.Width;
            oldTop = win.Top;
            oldLeft = win.Left;
            double offset = WpfScreenHelper.Screen.PrimaryScreen.WorkingArea.Height == SystemParameters.PrimaryScreenHeight ?
                0 : SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height;
            PostitionWindowOnScreen(win, 0, -offset / 2);
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