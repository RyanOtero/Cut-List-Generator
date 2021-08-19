using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Solidworks_Cutlist_Generator.Models;
using Solidworks_Cutlist_Generator.ViewModels.Commands;
using static Solidworks_Cutlist_Generator.Utils.Messenger;


namespace Solidworks_Cutlist_Generator.ViewModels {
    public class EditStockItemViewModel : ViewModelBase {

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

        public EditStockItemViewModel(StockItem sItem) {
            SelectedMatType = sItem.MatType;
            SelectedProfType = sItem.ProfType;
            StockLength = sItem.StockLength;
            InternalDescription = sItem.InternalDescription;
            ExternalDescription = sItem.ExternalDescription;
            CostPerFoot = sItem.CostPerFoot;
            SelectedVendor = sItem.Vendor;
            MainVModel = (MainViewModel)Application.Current.MainWindow.DataContext;
            ConfirmCommand = new RelayCommand((x) => {
                if (!string.IsNullOrEmpty(InternalDescription) && !string.IsNullOrEmpty(ExternalDescription) && SelectedVendor != null) {
                    if (SelectedVendor != sItem.Vendor || SelectedMatType != sItem.MatType || SelectedProfType != sItem.ProfType || CostPerFoot != sItem.CostPerFoot
                        || StockLength != sItem.StockLength || InternalDescription != sItem.InternalDescription || ExternalDescription != sItem.ExternalDescription) {
                        try {
                            sItem.MatType = SelectedMatType;
                            sItem.ProfType = SelectedProfType;
                            sItem.StockLength = StockLength;
                            sItem.InternalDescription = InternalDescription;
                            sItem.ExternalDescription = ExternalDescription;
                            sItem.CostPerFoot = CostPerFoot;
                            sItem.Vendor = SelectedVendor;
                            using (CutListGeneratorContext ctx = new CutListGeneratorContext(MainVModel.ConnectionString)) {
                                ctx.Entry(SelectedVendor).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                                ctx.StockItems.Update(sItem);
                                ctx.SaveChanges();
                            }
                            List<CutItem> tempList = MainVModel.CutListMngr.CutList.ToList();
                            MainVModel.ClearCutList();
                            MainVModel.CutListMngr.SortCutListForDisplay(MainVModel.IsDetailed, tempList);
                            MainVModel.CutListMngr.Refresh();
                            CloseWin(x);
                        } catch (Exception e) {
                            string s = e.Message;
                            ErrorMessage("Database Error", "There was an error while accessing the database.");
                        }
                    }
                    CloseWin(x);
                } else {
                    ErrorMessage("Empty Fields", "Fields cannot be empty. Please fill in the missing fields before confirming");
                }
            });
            CancelCommand = new RelayCommand(CloseWin);
        }
    }
}