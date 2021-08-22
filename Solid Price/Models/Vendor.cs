using static Solid_Price.Utils.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Solid_Price.Models {
    public class Vendor : IEquatable<Vendor>, INotifyPropertyChanged {

        #region Fields
        private string vendorName;
        private string phoneNumber;
        private string contactName;
        private string contactEmail;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
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
        #endregion

        #region Constructors        
        public Vendor(string vendorName = "", string phoneNumber = "", string contactName = "", string contactEmail = "") {
            VendorName = vendorName;
            PhoneNumber = phoneNumber;
            ContactName = contactName;
            ContactEmail = contactEmail;
        }

        public Vendor(Vendor other) {
            VendorName = other.vendorName;
            PhoneNumber = other.phoneNumber;
            ContactName = other.contactName;
            ContactEmail = other.contactEmail;
        }
        #endregion

        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Comparison
        public override bool Equals(object obj) => this.Equals(obj as Vendor);

        public override int GetHashCode() => ID.GetHashCode();

        public bool Equals(Vendor other) {
            if (other is null) {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other)) {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType()) {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (ID == other.ID)
                && (ContactName == other.ContactName)
                && (ContactEmail == other.ContactEmail)
                && (VendorName == other.VendorName);

        }

        public static bool operator ==(Vendor left, Vendor right) {
            if (left is null) {
                if (right is null) {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return left.Equals(right);
        }

        public static bool operator !=(Vendor left, Vendor right) => !(left == right);
    }
    #endregion
}
