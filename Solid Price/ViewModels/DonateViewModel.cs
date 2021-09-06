using SolidPrice.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolidPrice.ViewModels {
    class DonateViewModel : ViewModelBase {
        public RelayCommand CopyCommand { get; set; }

        public DonateViewModel() {
            CopyCommand = new RelayCommand(x => {
                string[] s = x.ToString().Split(" ");

                Clipboard.Clear();
                Clipboard.SetText(s[1]);
            });

        }
    }
}
