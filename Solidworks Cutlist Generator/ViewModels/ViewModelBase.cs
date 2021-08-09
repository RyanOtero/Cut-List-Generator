using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Solidworks_Cutlist_Generator.ViewModels {
    public abstract class ViewModelBase : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase() {

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "") {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        public void CloseWin(object obj) {
            Window win = obj as Window;
            win.Close();
        }
    }
}
