using SolidPrice.Models;
using SolidPrice.ViewModels.Commands;
using System;
using System.Windows;
using static SolidPrice.Utils.Messenger;


namespace SolidPrice.ViewModels {
    public class EditVendorViewModel : ViewModelBase {
        private string vendorName;
        private string phoneNumber;
        private string contactName;
        private string contactEmail;

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

        public EditVendorViewModel(Vendor vendor) {
            VendorName = vendor.VendorName;
            PhoneNumber = vendor.PhoneNumber;
            ContactName = vendor.ContactName;
            contactEmail = vendor.ContactEmail;
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            ConfirmCommand = new RelayCommand((x) => {
                if (!string.IsNullOrEmpty(VendorName) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(ContactName) && !string.IsNullOrEmpty(ContactEmail)) {
                    if (vendor.VendorName != VendorName || vendor.PhoneNumber != PhoneNumber || vendor.ContactName != ContactName || vendor.ContactEmail != ContactEmail) {
                        try {
                            vendor.VendorName = VendorName;
                            vendor.PhoneNumber = PhoneNumber;
                            vendor.ContactName = ContactName;
                            vendor.ContactEmail = ContactEmail;
                            using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                                ctx.Vendors.Update(vendor);
                                ctx.SaveChanges();
                            }
                            CutListManager.Instance.Refresh();
                            CloseWin(x);
                        } catch (Exception) {
                            ErrorMessage("Database Error evvm.cs 63", "There was an error while accessing the database.");
                        }
                    } else {
                        CloseWin(x);
                    }
                } else {
                    ErrorMessage("Empty Fields evvm.cs 69", "Fields cannot be empty. Please fill in the missing fields before confirming");
                }
            });
            CancelCommand = new RelayCommand(CloseWin);
        }
    }
}

