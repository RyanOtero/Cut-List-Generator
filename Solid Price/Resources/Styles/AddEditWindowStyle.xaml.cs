using System.Windows;
using System.Windows.Input;

namespace SolidPrice.Styles {
    public partial class AddEditWindowStyle : ResourceDictionary {

        public AddEditWindowStyle() {
            InitializeComponent();
        }

        /**
        * Header Buttons Events
        **/


        private void quitBtn_Click(object sender, RoutedEventArgs e) {
            Window win = Window.GetWindow(((FrameworkElement)e.Source));
            dynamic d = win.DataContext;
            d.CloseWin(win);
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