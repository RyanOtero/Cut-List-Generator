using SolidPrice.Models;
using SolidPrice.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static SolidPrice.Utils.Messenger;


namespace SolidPrice.ViewModels {
    public class EditStockItemViewModel : ViewModelBase {

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

        public EditStockItemViewModel(StockItem sItem) {
            SelectedMatType = sItem.MatType;
            SelectedProfType = sItem.ProfType;
            StockLength = sItem.StockLength;
            InternalDescription = sItem.InternalDescription;
            ExternalDescription = sItem.ExternalDescription;
            CostPerFoot = sItem.CostPerFoot;
            SelectedVendor = sItem.Vendor;
            VendorItemNumber = sItem.VendorItemNumber;
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            ConfirmCommand = new RelayCommand((x) => {
                if (!string.IsNullOrEmpty(InternalDescription) && !string.IsNullOrEmpty(ExternalDescription) && SelectedVendor != null) {
                    if (SelectedVendor != sItem.Vendor || SelectedMatType != sItem.MatType || SelectedProfType != sItem.ProfType || CostPerFoot != sItem.CostPerFoot
                        || StockLength != sItem.StockLength || InternalDescription != sItem.InternalDescription || ExternalDescription != sItem.ExternalDescription || sItem.VendorItemNumber != VendorItemNumber) {
                        try {
                            sItem.MatType = SelectedMatType;
                            sItem.ProfType = SelectedProfType;
                            sItem.StockLength = StockLength;
                            sItem.InternalDescription = InternalDescription;
                            sItem.ExternalDescription = ExternalDescription;
                            sItem.CostPerFoot = CostPerFoot;
                            sItem.Vendor = SelectedVendor;
                            sItem.VendorItemNumber = VendorItemNumber;
                            using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                                ctx.Entry(SelectedVendor).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                                ctx.StockItems.Update(sItem);
                                ctx.SaveChanges();
                            }
                            List<CutItem> tempCList = CutListManager.Instance.CutListDetailed.ToList();
                            List<CutItem> tempCListSimple = CutListManager.Instance.CutListDetailed.ToList();
                            List<SheetCutItem> tempSCList = CutListManager.Instance.SheetCutList.ToList();
                            MainVModel.ClearCutList();
                            CutListManager.Instance.SortCutListForDisplay( tempCList, tempCListSimple);
                            CutListManager.Instance.Refresh();
                            CloseWin(x);
                        } catch (Exception e) {
                            string s = e.Message;
                            ErrorMessage("Database Error esivm.cs 106", "There was an error while accessing the database.");
                        }
                    }
                    CloseWin(x);
                } else {
                    ErrorMessage("Empty Fields esivm.cs 111", "Fields cannot be empty. Please fill in the missing fields before confirming");
                }
            });
            CancelCommand = new RelayCommand(CloseWin);
        }
    }
}