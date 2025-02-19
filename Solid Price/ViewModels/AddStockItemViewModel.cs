﻿using SolidPrice.Models;
using SolidPrice.ViewModels.Commands;
using System;
using System.Linq;
using System.Windows;
using static SolidPrice.Utils.Messenger;


namespace SolidPrice.ViewModels {
    public class AddStockItemViewModel : ViewModelBase {

        #region Fields
        private MaterialType selectedMatType;
        private ProfileType selectedProfType;
        private float stockLength;
        private string internalDescription;
        private string externalDescription;
        private Vendor selectedVendor;
        private decimal costPerFoot;
        private string vendorItemNumber;
        #endregion

        #region Properties
        public MaterialType SelectedMatType {
            get => selectedMatType;
            set { SetProperty(ref selectedMatType, value); }
        }

        public ProfileType SelectedProfType {
            get => selectedProfType;
            set { SetProperty(ref selectedProfType, value); }
        }

        public float StockLength {
            get => stockLength;
            set { SetProperty(ref stockLength, value); }
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

        public decimal CostPerFoot {
            get => costPerFoot;
            set { SetProperty(ref costPerFoot, value); }
        }

        public string VendorItemNumber {
            get => vendorItemNumber;
            set { SetProperty(ref vendorItemNumber, value); }
        }

        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public MainViewModel MainVModel { get; set; }
        #endregion

        public AddStockItemViewModel() {
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
                            StockItem stockItem = new StockItem(SelectedVendor, selectedMatType, SelectedProfType, CostPerFoot, StockLength, InternalDescription, ExternalDescription, VendorItemNumber);
                            ctx.StockItems.Add(stockItem);
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