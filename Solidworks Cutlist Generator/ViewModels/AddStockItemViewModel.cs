using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Solidworks_Cutlist_Generator.Models;
using Solidworks_Cutlist_Generator.ViewModels.Commands;
using static Solidworks_Cutlist_Generator.Utils.Messenger;


namespace Solidworks_Cutlist_Generator.ViewModels {
    public class AddStockItemViewModel : ViewModelBase {

        #region Fields
        private MaterialType selectedMatType;
        private ProfileType selectedProfType;
        private float stockLength;
        private string internalDescription;
        private string externalDescription;
        private Vendor selectedVendor;
        private decimal costPerFoot;
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
                    ErrorMessage("Database Error", "There was an error while accessing the database.");
                    return;
                }
            }
            SelectedVendor = MainVModel.Vendors[0];
            ConfirmCommand = new RelayCommand((x) => {
                if (!string.IsNullOrEmpty(InternalDescription) && !string.IsNullOrEmpty(ExternalDescription) && SelectedVendor != null) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                            ctx.Entry(SelectedVendor).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                            StockItem stockItem = new StockItem( SelectedVendor, selectedMatType, SelectedProfType, CostPerFoot, StockLength, InternalDescription, ExternalDescription);
                            ctx.StockItems.Add(stockItem);
                            ctx.SaveChanges();
                        }
                        MainVModel.CutListMaker.Refresh();
                        CloseWin(x);
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error", "There was an error while accessing the database.");
                    }
                } else {
                    ErrorMessage("Empty Fields", "Fields cannot be empty. Please fill in the missing fields before confirming");
                }
            });
            CancelCommand = new RelayCommand(CloseWin);
        }
    }
}