using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidPrice.ViewModels {
    class ConfigViewModel : ViewModelBase {
        #region Fields
        private string serverString;
        private string databaseString;
        private string userString;
        private string passwordString;
        #endregion

        #region Properties
        public string ServerString {
            get => serverString;
            set {
                SetProperty(ref serverString, value);
                Application.Current.Properties["ServerString"] = serverString;
                if (MainVModel.CutListMngr != null) {
                    MainVModel.CutListMngr.ConnectionString = MainVModel.ConnectionString;
                }
            }
        }
        public string DatabaseString {
            get => databaseString;
            set {
                SetProperty(ref databaseString, value);
                Application.Current.Properties["DatabaseString"] = databaseString;
                if (MainVModel.CutListMngr != null) {
                    MainVModel.CutListMngr.ConnectionString = MainVModel.ConnectionString;
                }
            }
        }
        public string UserString {
            get => userString;
            set {
                SetProperty(ref userString, value);
                Application.Current.Properties["UserString"] = userString;
                if (MainVModel.CutListMngr != null) {
                    MainVModel.CutListMngr.ConnectionString = MainVModel.ConnectionString;
                }
            }
        }
        public string PasswordString {
            get => passwordString;
            set {
                SetProperty(ref passwordString, value);
                Application.Current.Properties["PasswordString"] = passwordString;
                if (MainVModel.CutListMngr != null) {
                    MainVModel.CutListMngr.ConnectionString = MainVModel.ConnectionString;
                }
            }
        }
        public MainViewModel MainVModel { get; set; }
        #endregion

        public ConfigViewModel() {
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;

        }

    }
}
