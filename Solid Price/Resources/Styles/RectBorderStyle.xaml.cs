using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace SolidPrice.Styles {
    public partial class RectBorderStyle : ResourceDictionary {

        public RectBorderStyle() {
            InitializeComponent();
        }

        bool ResizeInProcess = false;
        private void Resize_Init(object sender, MouseButtonEventArgs e) {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null) {
                ResizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e) {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null) {
                ResizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizing_Form(object sender, MouseEventArgs e) {
            if (ResizeInProcess) {
                Rectangle senderRect = sender as Rectangle;
                Window mainWindow = senderRect.Tag as Window;
                if (senderRect != null) {
                    double width = e.GetPosition(mainWindow).X;
                    double height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.ToLower().Contains("right")) {
                        width += 5;
                        if (width > mainWindow.MinWidth)
                            mainWindow.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("left")) {
                        width -= 5;
                        if (mainWindow.Width - width > mainWindow.MinWidth) {
                            mainWindow.Left += width;
                            width = mainWindow.Width - width;
                        }
                        if (width > mainWindow.MinWidth) {
                            mainWindow.Width = width;
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("bottom")) {
                        height += 5;
                        if (height > mainWindow.MinHeight)
                            mainWindow.Height = height;
                    }
                    if (senderRect.Name.ToLower().Contains("top")) {
                        height -= 5;
                        if (mainWindow.Height - height > mainWindow.MinHeight) {
                            mainWindow.Top += height;
                            height = mainWindow.Height - height;
                        }
                        if (height > mainWindow.MinHeight) {
                            mainWindow.Height = height;
                        }
                    }
                }
            }
        }
    }
}