﻿using SolidPrice.Models;
using SolidPrice.ViewModels.Commands;
using System;
using System.Linq;
using System.Windows;
using static SolidPrice.Utils.Messenger;


namespace SolidPrice.ViewModels {
    public class AddVendorViewModel : ViewModelBase {

        #region Fields
        private string vendorName;
        private string phoneNumber;
        private string contactName;
        private string contactEmail;
        #endregion

        #region Properties
        public string VendorName {
            get => vendorName;
            set { SetProperty(ref vendorName, value); }
        }

        public string PhoneNumber {
            get => phoneNumber;
            set { SetProperty(ref phoneNumber, value); }
        }

        public string ContactName {
            get => contactName;
            set { SetProperty(ref contactName, value); }
        }

        public string ContactEmail {
            get => contactEmail;
            set { SetProperty(ref contactEmail, value); }
        }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public MainViewModel MainVModel { get; set; }
        #endregion

        public AddVendorViewModel() {
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            ConfirmCommand = new RelayCommand((x) => {
                if (MainVModel.Vendors.Count == 0) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                            Vendor vendor;
                            if (ctx.Vendors.Any()) {
                                vendor = ctx.Vendors.AsEnumerable().ElementAt(0);
                            } else {
                                vendor = new Vendor("N/A", "N/A", "N/A", "N/A");
                                vendor.ID = 1;
                                ctx.Vendors.Add(vendor);
                                MainVModel.Vendors.Add(vendor);
                                ctx.SaveChanges();
                            }
                        }
                    } catch (Exception e) {
                        ErrorMessage("Database Error avvm.cs 65", "There was an error while accessing the database.");
                        return;
                    }
                }
                if (!string.IsNullOrEmpty(VendorName) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(ContactName) && !string.IsNullOrEmpty(ContactEmail)) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                            Vendor vendor = new Vendor(VendorName, PhoneNumber, ContactName, ContactEmail);
                            ctx.Vendors.Add(vendor);
                            ctx.SaveChanges();
                        }
                        CutListManager.Instance.Refresh();
                        CloseWin(x);
                    } catch (Exception) {
                        ErrorMessage("Database Error avvm.cs 79", "There was an error while accessing the database.");
                    }
                } else {
                    ErrorMessage("Empty Fields avvm.cs 82", "Fields cannot be empty. Please fill in the missing fields before confirming");
                }
            });
            CancelCommand = new RelayCommand(CloseWin);
        }
    }
}