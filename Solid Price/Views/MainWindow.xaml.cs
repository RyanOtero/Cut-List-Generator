using System.Windows;
using System.Windows.Input;
using SolidPrice.Styles;

namespace SolidPrice.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window {

        public MainWindow() {
            InitializeComponent();
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, System.EventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                MainWindowStyle.MaxState();
            }
        }
    }
}
