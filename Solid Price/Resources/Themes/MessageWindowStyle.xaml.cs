using System.Windows;

namespace Solid_Price.Themes {
    public partial class MessageWindowStyle : ResourceDictionary {

        public MessageWindowStyle() {
            InitializeComponent();
        }

        /**
        * Header Buttons Events
        **/


        private void quitBtn_Click(object sender, RoutedEventArgs e) {
            Window.GetWindow(((FrameworkElement)e.Source)).Close();
        }
    }
}