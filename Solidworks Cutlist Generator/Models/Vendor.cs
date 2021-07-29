﻿using static Solidworks_Cutlist_Generator.Utils.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Solidworks_Cutlist_Generator.Models {
    public class Vendor : IEquatable<Vendor>, INotifyPropertyChanged {
        private string vendorName;
        private string phoneNumber;
        private string contactName;
        private string contactEmail;
        private List<StockItem> stockItems;

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

        public virtual List<StockItem> StockItems {
            get => stockItems; set {
                stockItems = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        public Vendor(string vendorName = "", string phoneNumber = "", string contactName = "", string contactEmail = "") {
            VendorName = vendorName;
            PhoneNumber = phoneNumber;
            ContactName = contactName;
            ContactEmail = contactEmail;
            StockItems = new List<StockItem>();
        }

        public Vendor(Vendor other) {
            VendorName = other.vendorName;
            PhoneNumber = other.phoneNumber;
            ContactName = other.contactName;
            ContactEmail = other.contactEmail;
            StockItems = new List<StockItem>();

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
}
