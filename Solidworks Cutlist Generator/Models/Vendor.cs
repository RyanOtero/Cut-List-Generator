using static Solidworks_Cutlist_Generator.BusinessLogic.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Solidworks_Cutlist_Generator.Model {
    public class Vendor : INotifyPropertyChanged {
        private string vendorName;
        private string phoneNumber;
        private string contactName;
        private string contactEmail;

        public int ID { get; set; }
        public string VendorName {
            get => vendorName; set {
                vendorName = value;
                OnPropertyChanged();
            }
        }
        public string PhoneNumber {
            get => phoneNumber; set {
                phoneNumber = value;
                OnPropertyChanged();
            }
        }
        public string ContactName {
            get => contactName; set {
                contactName = value;
                OnPropertyChanged();
            }
        }
        public string ContactEmail {
            get => contactEmail; set {
                contactEmail = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Vendor() { }

        public Vendor(string vendorName = "", string phoneNumber = "", string contactName = "", string contactEmail = "") {
            VendorName = vendorName;
            PhoneNumber = phoneNumber;
            ContactName = contactName;
            ContactEmail = contactEmail;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
