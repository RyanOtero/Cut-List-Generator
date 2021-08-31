using SolidPrice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidPrice.ViewModels {
    class ConfigViewModel : ViewModelBase {

        public MainViewModel MainVModel { get; set; }

        public ConfigViewModel() {
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
        }

        public override void CloseWin(object obj) {
            CutListManager.Instance.Refresh();
            base.CloseWin(obj);
        }
    }
}
