using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Solidworks_Cutlist_Generator.Models;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Solidworks_Cutlist_Generator.ViewModels.Commands;
using static Solidworks_Cutlist_Generator.Utils.Messenger;
using Solidworks_Cutlist_Generator.Views;

namespace Solidworks_Cutlist_Generator.ViewModels {
    public class MainViewModel : ViewModelBase {

        #region Fields
        private readonly object asyncLock;
        private string sourceText;
        private string loadingText;
        private bool isDetailed;
        private bool showPricing;
        private bool isLoading;
        private ObservableCollection<Vendor> vendors;
        private ObservableCollection<StockItem> stockItems;
        private ObservableCollection<CutItem> cutList;
        private ObservableCollection<OrderItem> orderList;
        private bool atWork;
        private string orderTotal;
        private string cutListTotal;
        private Vendor selectedVendor;
        private StockItem selectedStockItem;
        private bool useExternalDB;
        private string serverString;
        private string databaseString;
        private string userString;
        private string passwordString;
        private string sqliteString = "FileName=CutList.db";
        #endregion

        #region Properties
        public bool UseExternalDB {
            get => useExternalDB;
            set {
                SetProperty(ref useExternalDB, value);
                Application.Current.Properties["UseExternalDB"] = useExternalDB;
                if (CutListMngr != null) {
                    CutListMngr.ConnectionString = ConnectionString;
                    if (ConnectionString != "server=;database=;user=;password=") {
                        CutListMngr.Refresh();
                    }
                }
            }
        }

        public string ServerString {
            get => serverString;
            set {
                SetProperty(ref serverString, value);
                Application.Current.Properties["ServerString"] = serverString;
                if (CutListMngr != null) {
                    CutListMngr.ConnectionString = ConnectionString;
                }
            }
        }
        public string DatabaseString {
            get => databaseString;
            set {
                SetProperty(ref databaseString, value);
                Application.Current.Properties["DatabaseString"] = databaseString;
                if (CutListMngr != null) {
                    CutListMngr.ConnectionString = ConnectionString;
                }
            }
        }
        public string UserString {
            get => userString;
            set {
                SetProperty(ref userString, value);
                Application.Current.Properties["UserString"] = userString;
                if (CutListMngr != null) {
                    CutListMngr.ConnectionString = ConnectionString;
                }
            }
        }
        public string PasswordString {
            get => passwordString;
            set {
                SetProperty(ref passwordString, value);
                Application.Current.Properties["PasswordString"] = passwordString;
                if (CutListMngr != null) {
                    CutListMngr.ConnectionString = ConnectionString;
                }
            }
        }

        public string ExternalConnectionString {
            get => "server=" + ServerString
                + ";database=" + DatabaseString
                + ";user=" + UserString
                + ";password=" + PasswordString;
        }

        public string ConnectionString {
            get {
                if (UseExternalDB) {
                    return ExternalConnectionString;
                } else {
                    return sqliteString;
                }
            }
            set {
                if (CutListMngr != null && CutListMngr.ConnectionString != value) {
                    CutListMngr.ConnectionString = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Command Props
        public RelayCommand GenerateCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand ClearCommand { get; set; }

        public RelayCommand SourceBrowseCommand { get; set; }

        public RelayCommand RefreshCommand { get; set; }

        public RelayCommand SaveCStringCommand { get; set; }

        public RelayCommand AddStockItemCommand { get; set; }

        public RelayCommand DeleteStockItemCommand { get; set; }

        public RelayCommand EditStockItemCommand { get; set; }

        public RelayCommand AddVendorCommand { get; set; }

        public RelayCommand DeleteVendorCommand { get; set; }

        public RelayCommand EditVendorCommand { get; set; }

        public RelayCommand IsDetailedCommand { get; set; }
        #endregion


        public CutListManager CutListMngr { get; set; }

        public Vendor SelectedVendor {
            get => selectedVendor;
            set { SetProperty(ref selectedVendor, value); }
        }

        public StockItem SelectedStockItem {
            get => selectedStockItem;
            set { SetProperty(ref selectedStockItem, value); }
        }
        public bool IsDetailed {
            get => isDetailed;
            set { SetProperty(ref isDetailed, value); }
        }

        public bool IsLoading {
            get => isLoading;
            set { SetProperty(ref isLoading, value); }
        }

        public bool ShowPricing {
            get => showPricing;
            set { SetProperty(ref showPricing, value); }
        }

        public string SourceText {
            get => sourceText;
            set {
                SetProperty(ref sourceText, value);
            }
        }

        public string LoadingText {
            get => loadingText;
            set {
                SetProperty(ref loadingText, value);
            }
        }

        public string OrderTotal {
            get => orderTotal;
            set {
                SetProperty(ref orderTotal, value);
            }
        }

        public string CutListTotal {
            get => cutListTotal;
            set {
                SetProperty(ref cutListTotal, value);
            }
        }

        public ObservableCollection<Vendor> Vendors {
            get => vendors;
            set { SetProperty(ref vendors, value); }
        }

        public ObservableCollection<StockItem> StockItems {
            get => stockItems;
            set { SetProperty(ref stockItems, value); }
        }

        public ObservableCollection<CutItem> CutList {
            get => cutList;
            set {
                SetProperty(ref cutList, value);
                BindingOperations.EnableCollectionSynchronization(cutList, asyncLock);
            }
        }

        public ObservableCollection<OrderItem> OrderList {
            get => orderList;
            set {
                SetProperty(ref orderList, value);
                BindingOperations.EnableCollectionSynchronization(orderList, asyncLock);
            }
        }
        #endregion

        public MainViewModel() {
            UseExternalDB = bool.Parse(Application.Current.Properties["UseExternalDB"].ToString());
            ServerString = Application.Current.Properties["ServerString"].ToString();
            DatabaseString = Application.Current.Properties["DatabaseString"].ToString();
            UserString = Application.Current.Properties["UserString"].ToString();
            PasswordString = Application.Current.Properties["PasswordString"].ToString();

            #region Commands Set
            GenerateCommand = new RelayCommand((x) => GenerateCutList(), () => !string.IsNullOrEmpty(SourceText));
            SaveCommand = new RelayCommand((x) => SaveCutList(), () => CutList != null && CutList.Count > 0);
            ClearCommand = new RelayCommand((x) => ClearCutList(), () => CutList != null && CutList.Count > 0);
            SourceBrowseCommand = new RelayCommand((x) => SourceBrowse());
            RefreshCommand = new RelayCommand((x) => {
                CutListMngr.ConnectionString = ConnectionString; CutListMngr.Refresh();
                GetTotalText();
            });
            AddStockItemCommand = new RelayCommand((x) => {
                var win = new AddStockItemWindow();
                win.DataContext = new AddStockItemViewModel();
                win.Show();
            });
            AddVendorCommand = new RelayCommand((x) => {
                var win = new AddVendorWindow();
                win.DataContext = new AddVendorViewModel();
                win.Show();
            });
            DeleteStockItemCommand = new RelayCommand((x) => {
                if (SelectedStockItem == null) {
                    return;
                }

                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show("Are you sure you want to delete this stock type from the database?", "Delete Stock Type", button, icon);
                if (result == MessageBoxResult.Yes) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {


                            ctx.StockItems.Remove(SelectedStockItem);
                            ctx.SaveChanges();
                            CutListMngr.Refresh();
                        }
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error", "There was an error while accessing the database.");
                    }
                }
            });
            DeleteVendorCommand = new RelayCommand((x) => {
                if (SelectedVendor == null) {
                    return;
                }
                if (SelectedVendor.ID == 1) {
                    ErrorMessage("Forbidden Action", "You may not delete the default vendor.");
                    return;
                }

                List<StockItem> sItems = StockItems.Where(s => s.Vendor == SelectedVendor).ToList();

                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                string msg = "Are you sure you want to delete this vendor from the database?";
                if (sItems.Count > 0) {
                    msg = "There are Stock Types associated with this vendor. If this vendor is " +
                    "deleted, thier vendor will be set to the default vendor. " +
                    "Are you sure you want to delete this vendor from the database?";
                }

                result = MessageBox.Show(msg, "Delete Vendor", button, icon);
                if (result == MessageBoxResult.Yes) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {

                            Vendor defaultVendor = Vendors.Single(v => v.ID == 1);
                            ctx.Vendors.Remove(SelectedVendor);
                            foreach (StockItem item in sItems) {
                                item.Vendor = defaultVendor;
                            }
                            ctx.StockItems.UpdateRange(sItems);
                            ctx.SaveChanges();
                            CutListMngr.Refresh();
                        }
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error", "There was an error while accessing the database.");
                    }
                }
            });
            EditStockItemCommand = new RelayCommand((x) => {
                if (SelectedStockItem == null) {
                    ErrorMessage("Stock Type", "Please select a stock type to edit.");
                    return;
                }
                var vModel = new EditStockItemViewModel(SelectedStockItem);
                var win = new EditStockItemWindow();
                win.DataContext = vModel;
                win.Show();
            });
            EditVendorCommand = new RelayCommand((x) => {
                if (SelectedVendor == null) {
                    ErrorMessage("Vender", "Please select a vendor to edit.");
                    return;
                }
                if (SelectedVendor.ID == 1) {
                    ErrorMessage("Vendor", "You may not edit the default vendor.");
                    return;
                }
                var vModel = new EditVendorViewModel(SelectedVendor);
                var win = new EditVendorWindow();
                win.DataContext = vModel;
                win.Show();
            });
            IsDetailedCommand = new RelayCommand((x) => {
                List<CutItem> tempList = CutListMngr.CutList.ToList();
                ClearCutList();
                CutListMngr.SortCutListForDisplay(IsDetailed, tempList);
                CutListMngr.Refresh();
            });
            #endregion

            asyncLock = new object();
            CutListMngr = new CutListManager(ConnectionString);
            CutList = CutListMngr.CutList;
            OrderList = CutListMngr.OrderList;
            Vendors = CutListMngr.Vendors;
            StockItems = CutListMngr.StockItems;
            IsLoading = false;
            LoadingText = "Loading...";
            OrderTotal = "Total:";
            CutListTotal = "Total:";

            ////For testing
            //atWork = false;
            //if (atWork) {
            //    SourceText = @"D:\Projects\2021\1-Aluminum Awnings\1-0010 SL2 Consulting LLC Zephyrhills, FL\Drawing\SolidWorks\G-Gutter 1\G-Gutter 1.SLDASM";
            //} else {
            //    SourceText = @"C:\Users\Ryan\Desktop\Drawing\Wooden Structures\Pergola - 16ft x 11ft.SLDPRT";
            //}

            GetTotalText();
        }

        #region Methods
        private void GetTotalText() {
            decimal runningTotal = 0;
            foreach (OrderItem item in OrderList) {
                runningTotal += decimal.Parse(item.TotalCost.Substring(1));
            }
            OrderTotal = "Total: " + string.Format("{0:c}", runningTotal);
            runningTotal = 0;
            foreach (CutItem item in CutList) {
                runningTotal += decimal.Parse(item.TotalCost.Substring(1));
            }
            CutListTotal = "Total: " + string.Format("{0:c}", runningTotal);
        }

        #endregion

        #region Button Clicks

        public async void GenerateCutList() {
            if (string.IsNullOrEmpty(ConnectionString)) {
                ErrorMessage("Database Error", "There was an error while accessing the database.");
                return;
            }
            IsLoading = true;
            await Task.Run(() => CutListMngr.Generate(SourceText, IsDetailed));
            CutListMngr.Refresh();
            IsLoading = false;
            GetTotalText();
        }

        public void SourceBrowse() {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
                SourceText = openFileDialog.FileName;
        }

        public void SaveCutList() {
            string filePath;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel File (*.xlsx)|*.xlsx|Comma-Separated Values (*.csv)|*.csv|All Files (*.*)|*.*";
            saveFileDialog.Title = "Export Cut List";
            saveFileDialog.DefaultExt = "xlsx";
            if (saveFileDialog.ShowDialog() == true) {
                filePath = saveFileDialog.FileName;
                if (filePath.Contains(".xlsx")) {
                    GenerateExcel(ToDataTable(CutListMngr.CutList), filePath);
                } else if (filePath.Contains(".csv")) {
                    GenerateCSV(ToDataTable(CutListMngr.CutList), filePath);
                }
            }
        }

        public void ClearCutList() {
            CutListMngr.NewCutList();
        }
        #endregion

        #region Export Functionality

        public static DataTable ToDataTable<T>(ObservableCollection<T> items) {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties) {
                //Defining type of data column gives proper data table 
                var type = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType;
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (var item in items) {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++) {
                    //inserting property values to data table rows
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check data table
            return dataTable;
        }

        private void GenerateExcel(DataTable DtIN, string filePath) {
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            Excel.Range cellRange;
            try {
                Excel.Application excel = new Excel.Application();
                excel.DisplayAlerts = false;
                excel.Visible = false;
                workBook = excel.Workbooks.Add(Type.Missing);
                workSheet = (Excel.Worksheet)workBook.ActiveSheet;
                workSheet.Name = "Cut List";
                DataTable tempDt = DtIN;
                workSheet.Cells.Font.Size = 11;
                int rowcount = 1;
                for (int i = 2; i <= tempDt.Columns.Count; i++) //taking care of Headers.  
                {
                    workSheet.Cells[1, i - 1] = tempDt.Columns[i - 1].ColumnName;
                }
                foreach (DataRow row in tempDt.Rows) //taking care of each Row  
                    {
                    rowcount += 1;
                    for (int i = 1; i < tempDt.Columns.Count; i++) //taking care of each column  
                    {
                        workSheet.Cells[rowcount, i] = row[i].ToString();
                    }
                }
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[rowcount, tempDt.Columns.Count]];
                cellRange.EntireColumn.AutoFit();
                workBook.SaveAs(filePath, Excel.XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
            } catch (Exception) {
                throw;
            }
        }

        private void GenerateCSV(DataTable dataTable, string filePath) {
            FileStream fs = null;
            try {
                fs = new FileStream(filePath, FileMode.Create);
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) {
                    foreach (DataColumn col in dataTable.Columns) {
                        if (col == dataTable.Columns[0]) {
                            continue;
                        }
                        writer.Write(col.ColumnName + ",");
                    }
                    writer.Write("\n");
                    foreach (DataRow row in dataTable.Rows) {
                        foreach (var item in row.ItemArray) {
                            if (item == row.ItemArray[0]) {
                                continue;
                            }
                            writer.Write(item.ToString() + ",");
                        }
                        writer.Write("\n");
                    }
                }
            } catch (Exception) {
                MessageBox.Show("Unable to save file, try again.", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                if (fs != null) {
                    fs.Dispose();
                }
            }
        }

        #endregion
    }
}
