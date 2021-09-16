using SolidPrice.Models;
using SolidPrice.ViewModels.Commands;
using System;
using System.Linq;
using System.Windows;
using static SolidPrice.Utils.Messenger;


namespace SolidPrice.ViewModels {
    public class AddSheetStockItemViewModel : ViewModelBase {

        #region Fields
        private MaterialType selectedMatType;
        private float stockLength;
        private float stockWidth;
        private float thickness;
        private string finish;
        private string internalDescription;
        private string externalDescription;
        private Vendor selectedVendor;
        private decimal costPerSqFoot;
        private string vendorItemNumber;
        #endregion

        #region Properties
        public MaterialType SelectedMatType {
            get => selectedMatType;
            set { SetProperty(ref selectedMatType, value); }
        }

        public float StockLength {
            get => stockLength;
            set { SetProperty(ref stockLength, value); }
        }

        public float StockWidth {
            get => stockWidth;
            set { SetProperty(ref stockWidth, value); }
        }

        public float Thickness {
            get => thickness;
            set { SetProperty(ref thickness, value); }
        }

        public string Finish {
            get => finish;
            set { SetProperty(ref finish, value); }
        }

        public string InternalDescription {
            get => internalDescription;
            set { SetProperty(ref internalDescription, value); }
        }

        public string ExternalDescription {
            get => externalDescription;
            set { SetProperty(ref externalDescription, value); }
        }

        public Vendor SelectedVendor {
            get => selectedVendor;
            set { SetProperty(ref selectedVendor, value); }
        }

        public decimal CostPerSqFoot {
            get => costPerSqFoot;
            set { SetProperty(ref costPerSqFoot, value); }
        }

        public string VendorItemNumber {
            get => vendorItemNumber;
            set { SetProperty(ref vendorItemNumber, value); }
        }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public MainViewModel MainVModel { get; set; }
        #endregion

        public AddSheetStockItemViewModel() {
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
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
                    ErrorMessage("Database Error asivm.cs 88", "There was an error while accessing the database.");
                    return;
                }
            }
            SelectedVendor = MainVModel.Vendors[0];
            ConfirmCommand = new RelayCommand((x) => {
                if (!string.IsNullOrEmpty(InternalDescription) && !string.IsNullOrEmpty(ExternalDescription) && SelectedVendor != null) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                            ctx.Entry(SelectedVendor).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                            SheetStockItem sheetStockItem = new SheetStockItem(SelectedVendor, selectedMatType, CostPerSqFoot, StockLength, StockWidth, Thickness, Finish, InternalDescription, ExternalDescription, VendorItemNumber);
                            ctx.SheetStockItems.Add(sheetStockItem);
                            ctx.SaveChanges();
                        }
                        CutListManager.Instance.Refresh();
                        CloseWin(x);
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error asivm.cs 106", "There was an error while accessing the database.");
                    }
                } else {
                    ErrorMessage("Empty Fields asivm.cs 109", "Fields cannot be empty. Please fill in the missing fields before confirming");
                }
            });
            CancelCommand = new RelayCommand(CloseWin);
        }
    }
}