using SolidPrice.Models;
using SolidPrice.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static SolidPrice.Utils.Messenger;


namespace SolidPrice.ViewModels {
    public class EditSheetStockItemViewModel : ViewModelBase {

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

        public EditSheetStockItemViewModel(SheetStockItem sSItem) {
            SelectedMatType = sSItem.MatType;
            StockLength = sSItem.StockLengthInInches;
            StockWidth = sSItem.StockWidthInInches;
            Thickness = sSItem.Thickness;
            Finish = sSItem.Finish;
            InternalDescription = sSItem.InternalDescription;
            ExternalDescription = sSItem.ExternalDescription;
            CostPerSqFoot = sSItem.CostPerSqFoot;
            SelectedVendor = sSItem.Vendor;
            VendorItemNumber = sSItem.VendorItemNumber;
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            ConfirmCommand = new RelayCommand((x) => {
                if (!string.IsNullOrEmpty(InternalDescription) && !string.IsNullOrEmpty(ExternalDescription) && SelectedVendor != null) {
                    if (SelectedVendor != sSItem.Vendor || SelectedMatType != sSItem.MatType || Thickness != sSItem.Thickness || CostPerSqFoot != sSItem.CostPerSqFoot
                        || StockLength != sSItem.StockLengthInInches || StockWidth != sSItem.StockWidthInInches || Thickness != sSItem.Thickness || Finish != sSItem.Finish || InternalDescription != sSItem.InternalDescription 
                        || ExternalDescription != sSItem.ExternalDescription || sSItem.VendorItemNumber != VendorItemNumber) {
                        try {
                            sSItem.MatType = SelectedMatType;
                            sSItem.StockLengthInInches = StockLength;
                            sSItem.StockWidthInInches = StockWidth;
                            sSItem.Thickness = Thickness;
                            sSItem.Finish = Finish;
                            sSItem.InternalDescription = InternalDescription;
                            sSItem.ExternalDescription = ExternalDescription;
                            sSItem.CostPerSqFoot = CostPerSqFoot;
                            sSItem.Vendor = SelectedVendor;
                            sSItem.VendorItemNumber = VendorItemNumber;
                            using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                                ctx.Entry(SelectedVendor).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                                ctx.SheetStockItems.Update(sSItem);
                                ctx.SaveChanges();
                            }
                            List<CutItem> tempCList = CutListManager.Instance.CutListDetailed.ToList();
                            List<CutItem> tempCListSimple = CutListManager.Instance.CutListDetailed.ToList();

                            List<SheetCutItem> tempSCList = CutListManager.Instance.SheetCutList.ToList();
                            MainVModel.ClearCutList();
                            CutListManager.Instance.SortCutListForDisplay(tempCList, tempCListSimple);
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