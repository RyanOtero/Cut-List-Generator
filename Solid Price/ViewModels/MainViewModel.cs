using Microsoft.Win32;
using SolidPrice.Models;
using SolidPrice.Resources.Views;
using SolidPrice.ViewModels.Commands;
using SolidPrice.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using static SolidPrice.Utils.Messenger;
using Excel = Microsoft.Office.Interop.Excel;
using MessageBoxImage = SolidPrice.Models.MessageBoxImage;

namespace SolidPrice.ViewModels {
    public class MainViewModel : ViewModelBase {

        #region Fields
        private readonly object asyncLock;
        private string sourceText;
        private bool isDetailed;
        private bool showPricing;
        private ObservableCollection<Vendor> vendors;
        private ObservableCollection<StockItem> stockItems;
        private ObservableCollection<CutItem> cutList;
        private ObservableCollection<OrderItem> orderList;
        private ObservableCollection<SheetStockItem> sheetStockItems;
        private ObservableCollection<SheetCutItem> sheetCutList;
        private ObservableCollection<SheetOrderItem> sheetOrderList;
        //private bool atWork;
        private string orderTotal;
        private string sheetOrderTotal;
        private string cutListTotal;
        private string sheetCutListTotal;
        private Vendor selectedVendor;
        private StockItem selectedStockItem;
        private SheetStockItem selectedSheetStockItem;
        private bool useExternalDB;
        private string serverString;
        private string databaseString;
        private string userString;
        private string passwordString;
        Storyboard loadingAnimFadeIn;
        Storyboard loadingAnimFadeOut;
        Storyboard loadingAnimBounce;
        private bool isWorking;

        #endregion

        #region Properties
        public bool UseExternalDB {
            get => useExternalDB;
            set {
                SetProperty(ref useExternalDB, value);
                Application.Current.Properties["UseExternalDB"] = useExternalDB;
                if (CutListManager.Instance != null) {
                    CutListManager.Instance.ConnectionString = ConnectionString;
                    if (!value) CutListManager.Instance.Refresh();
                }
            }
        }

        public string ServerString {
            get => serverString;
            set {
                SetProperty(ref serverString, value);
                Application.Current.Properties["ServerString"] = serverString;
                if (CutListManager.Instance != null) {
                    CutListManager.Instance.ConnectionString = ConnectionString;
                }
            }
        }

        public string DatabaseString {
            get => databaseString;
            set {
                SetProperty(ref databaseString, value);
                Application.Current.Properties["DatabaseString"] = databaseString;
                if (CutListManager.Instance != null) {
                    CutListManager.Instance.ConnectionString = ConnectionString;
                }
            }
        }
        
        public string UserString {
            get => userString;
            set {
                SetProperty(ref userString, value);
                Application.Current.Properties["UserString"] = userString;
                if (CutListManager.Instance != null) {
                    CutListManager.Instance.ConnectionString = ConnectionString;
                }
            }
        }
        
        public string PasswordString {
            get => passwordString;
            set {
                SetProperty(ref passwordString, value);
                Application.Current.Properties["PasswordString"] = passwordString;
                if (CutListManager.Instance != null) {
                    CutListManager.Instance.ConnectionString = ConnectionString;
                }
            }
        }

        public string ExternalConnectionString {
            get => "server=" + ServerString
                + ";database=" + DatabaseString
                + ";user=" + UserString
                + ";password=" + PasswordString + ";";
        }

        public string ConnectionString {
            get {
                if (UseExternalDB) {
                    return ExternalConnectionString;
                } else {
                    return @"Data Source=C:\ProgramData\Solid Price\CutList.db";
                }
            }
            set {
                if (CutListManager.Instance != null && CutListManager.Instance.ConnectionString != value) {
                    CutListManager.Instance.ConnectionString = value;
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

        public RelayCommand AddSheetStockItemCommand { get; set; }

        public RelayCommand DeleteSheetStockItemCommand { get; set; }

        public RelayCommand EditSheetStockItemCommand { get; set; }

        public RelayCommand AddVendorCommand { get; set; }

        public RelayCommand DeleteVendorCommand { get; set; }

        public RelayCommand EditVendorCommand { get; set; }

        public RelayCommand IsDetailedCommand { get; set; }

        public RelayCommand ConfigCommand { get; set; }

        public RelayCommand DonateCommand { get; set; }

        #endregion

        public Vendor SelectedVendor {
            get => selectedVendor;
            set { SetProperty(ref selectedVendor, value); }
        }

        public StockItem SelectedStockItem {
            get => selectedStockItem;
            set { SetProperty(ref selectedStockItem, value); }
        }

        public SheetStockItem SelectedSheetStockItem {
            get => selectedSheetStockItem;
            set { SetProperty(ref selectedSheetStockItem, value); }
        }

        public bool IsDetailed {
            get => isDetailed;
            set { SetProperty(ref isDetailed, value); }
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

        public string SheetOrderTotal {
            get => sheetOrderTotal;
            set {
                SetProperty(ref sheetOrderTotal, value);
            }
        }

        public string SheetCutListTotal {
            get => sheetCutListTotal;
            set {
                SetProperty(ref sheetCutListTotal, value);
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

        public ObservableCollection<SheetStockItem> SheetStockItems {
            get => sheetStockItems;
            set { SetProperty(ref sheetStockItems, value); }
        }

        public ObservableCollection<SheetCutItem> SheetCutList {
            get => sheetCutList;
            set {
                SetProperty(ref sheetCutList, value);
                BindingOperations.EnableCollectionSynchronization(sheetCutList, asyncLock);
            }
        }

        public ObservableCollection<SheetOrderItem> SheetOrderList {
            get => sheetOrderList;
            set {
                SetProperty(ref sheetOrderList, value);
                BindingOperations.EnableCollectionSynchronization(sheetOrderList, asyncLock);
            }
        }

        public bool IsWorking {
            get => isWorking;
            set {
                SetProperty(ref isWorking, value);
            }
        }
        #endregion

        public MainViewModel() {
            IsWorking = false;
            UseExternalDB = bool.Parse(Application.Current.Properties["UseExternalDB"].ToString());
            ServerString = Application.Current.Properties["ServerString"].ToString();
            DatabaseString = Application.Current.Properties["DatabaseString"].ToString();
            UserString = Application.Current.Properties["UserString"].ToString();
            PasswordString = Application.Current.Properties["PasswordString"].ToString();

            #region Commands Set
            GenerateCommand = new RelayCommand(x => { 
                IsWorking = true;
                GenerateCutList();
            }, () => !string.IsNullOrEmpty(SourceText) && !IsWorking);
            SaveCommand = new RelayCommand(x => SaveCutList(), () => CutList != null && CutList.Count > 0 && !IsWorking);
            ClearCommand = new RelayCommand(x => ClearCutList(), () => CutList != null && CutList.Count > 0 && !IsWorking);
            SourceBrowseCommand = new RelayCommand(x => SourceBrowse(), () => !IsWorking);
            RefreshCommand = new RelayCommand(x => {
                CutListManager.Instance.ConnectionString = ConnectionString;
                CutListManager.Instance.Refresh();
                GetTotalText();
            }, () => !IsWorking);
            AddStockItemCommand = new RelayCommand(x => {
                var win = new AddStockItemWindow();
                win.DataContext = new AddStockItemViewModel();
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !IsWorking);
            AddSheetStockItemCommand = new RelayCommand(x => {
                var win = new AddSheetStockItemWindow();
                win.DataContext = new AddSheetStockItemViewModel();
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !IsWorking);

            AddVendorCommand = new RelayCommand(x => {
                var win = new AddVendorWindow();
                win.DataContext = new AddVendorViewModel();
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !isWorking);
            DeleteStockItemCommand = new RelayCommand(x => {
                if (SelectedStockItem == null) {
                    return;
                }

                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageWindow.Show("Delete Stock Type", "Are you sure you want to delete this stock type from the database?", button, icon);
                if (result == MessageBoxResult.Yes) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {


                            ctx.StockItems.Remove(SelectedStockItem);
                            ctx.SaveChanges();
                            CutListManager.Instance.Refresh();
                        }
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error mvm.cs 282", "There was an error while accessing the database.");
                    }
                }
            }, () => !IsWorking);
            DeleteSheetStockItemCommand = new RelayCommand(x => {
                if (SelectedSheetStockItem == null) {
                    return;
                }

                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageWindow.Show("Delete Sheet Type", "Are you sure you want to delete this sheet type from the database?", button, icon);
                if (result == MessageBoxResult.Yes) {
                    try {
                        using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {


                            ctx.SheetStockItems.Remove(SelectedSheetStockItem);
                            ctx.SaveChanges();
                            CutListManager.Instance.Refresh();
                        }
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error mvm.cs 282", "There was an error while accessing the database.");
                    }
                }
            }, () => !IsWorking);
            DeleteVendorCommand = new RelayCommand(x => {
                if (SelectedVendor == null) {
                    return;
                }
                if (SelectedVendor.ID == 1) {
                    ErrorMessage("Forbidden Action mvm.cs 291", "You may not delete the default vendor.");
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

                result = MessageWindow.Show("Delete Vendor", msg, button, icon);
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
                            CutListManager.Instance.Refresh();
                        }
                    } catch (Exception e) {
                        string s = e.Message;
                        ErrorMessage("Database Error mvm.cs 324", "There was an error while accessing the database.");
                    }
                }
            }, () => !IsWorking);
            EditStockItemCommand = new RelayCommand(x => {
                if (SelectedStockItem == null) {
                    ErrorMessage("Stock Type mvm.cs 330", "Please select a stock type to edit.");
                    return;
                }
                var vModel = new EditStockItemViewModel(SelectedStockItem);
                var win = new EditStockItemWindow();
                win.DataContext = vModel;
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !IsWorking);
            EditSheetStockItemCommand = new RelayCommand(x => {
                if (SelectedSheetStockItem == null) {
                    ErrorMessage("Stock Type mvm.cs 330", "Please select a sheet type to edit.");
                    return;
                }
                var vModel = new EditSheetStockItemViewModel(SelectedSheetStockItem);
                var win = new EditSheetStockItemWindow();
                win.DataContext = vModel;
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !IsWorking);

            EditVendorCommand = new RelayCommand(x => {
                if (SelectedVendor == null) {
                    ErrorMessage("Vender mvm.cs 340", "Please select a vendor to edit.");
                    return;
                }
                if (SelectedVendor.ID == 1) {
                    ErrorMessage("Vendor mvm.cs 344", "You may not edit the default vendor.");
                    return;
                }
                var vModel = new EditVendorViewModel(SelectedVendor);
                var win = new EditVendorWindow();
                win.DataContext = vModel;
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !IsWorking);
            IsDetailedCommand = new RelayCommand(x => {
                List<CutItem> tempCList = CutListManager.Instance.CutList.ToList();
                List<SheetCutItem> tempSCList = CutListManager.Instance.SheetCutList.ToList();
                ClearCutList();
                CutListManager.Instance.SortCutListForDisplay(IsDetailed, tempCList, tempSCList);
                //CutListManager.Instance.Refresh();
            });
            ConfigCommand = new RelayCommand(x => {
                var vModel = new ConfigViewModel();
                var win = new ConfigWindow();
                win.DataContext = vModel;
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            }, () => !IsWorking);
            DonateCommand = new RelayCommand(x => {
                var vModel = new DonateViewModel();
                var win = new DonateWindow();
                win.DataContext = vModel;
                Window mainWindow = App.Current.MainWindow;
                win.Left = mainWindow.Left + (mainWindow.Width - win.Width) / 2;
                win.Top = mainWindow.Top + (mainWindow.Height - win.Height) / 2;
                win.ShowDialog();
            });
            #endregion

            asyncLock = new object();
            CutListManager.Instance.ConnectionString = ConnectionString;
            CutList = CutListManager.Instance.CutList;
            SheetCutList = CutListManager.Instance.SheetCutList;
            OrderList = CutListManager.Instance.OrderList;
            SheetOrderList = CutListManager.Instance.SheetOrderList;
            StockItems = CutListManager.Instance.StockItems;
            SheetStockItems = CutListManager.Instance.SheetStockItems;
            Vendors = CutListManager.Instance.Vendors;
            OrderTotal = "";
            SheetOrderTotal = "";
            CutListTotal = "";
            SheetCutListTotal = "";
            loadingAnimFadeIn = (Storyboard)App.Current.MainWindow.FindResource("LoadingAnimFadeIn");
            loadingAnimBounce = (Storyboard)App.Current.MainWindow.FindResource("LoadingAnimBounce");
            loadingAnimFadeOut = (Storyboard)App.Current.MainWindow.FindResource("LoadingAnimFadeOut");
            loadingAnimFadeOut.Completed += new EventHandler((s, e) => { loadingAnimBounce.Stop(); });

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

        public override void CloseWin(object obj) {
            if (CutListManager.Instance.SWApp != null) {
                Task.Run(() => {
                    CutListManager.Instance.SWApp.ExitApp();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(CutListManager.Instance.SWApp);
                    CutListManager.Instance.SWApp = null;
                });
            }
            base.CloseWin(obj);
        }

        private void GetTotalText() {
            decimal runningTotal = 0;
            foreach (OrderItem item in OrderList) {
                runningTotal += decimal.Parse(item.TotalCost.Substring(1));
            }
            OrderTotal = string.Format("{0:c}", runningTotal);
            runningTotal = 0;
            foreach (CutItem item in CutList) {
                runningTotal += decimal.Parse(item.TotalCost.Substring(1));
            }
            CutListTotal = string.Format("{0:c}", runningTotal);
            runningTotal = 0;
            foreach (SheetOrderItem item in SheetOrderList) {
                runningTotal += decimal.Parse(item.TotalCost.Substring(1));
            }
            SheetOrderTotal = string.Format("{0:c}", runningTotal);
            runningTotal = 0;
            foreach (SheetCutItem item in SheetCutList) {
                runningTotal += decimal.Parse(item.TotalCost.Substring(1));
            }
            SheetCutListTotal = string.Format("{0:c}", runningTotal);
        }

        #endregion

        #region Button Clicks

        public async void GenerateCutList() {
            if (string.IsNullOrEmpty(ConnectionString)) {
                ErrorMessage("Database Error mvm.cs 402", "There was an error while accessing the database.");
                IsWorking = false;
                return;
            }
            ClearCutList();
            loadingAnimFadeIn.Begin();
            loadingAnimBounce.Begin();
            await Task.Run(() => CutListManager.Instance.Generate(SourceText, IsDetailed));
            loadingAnimFadeOut.Begin();
            GetTotalText();
            IsWorking = false;
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
                    GenerateExcel(ToDataTable(CutListManager.Instance.OrderList), ToDataTable(CutListManager.Instance.CutList), filePath);
                } else if (filePath.Contains(".csv")) {
                    GenerateCSV(ToDataTable(CutListManager.Instance.OrderList), ToDataTable(CutListManager.Instance.CutList), filePath);
                }
            }
        }

        public void ClearCutList() {
            CutListManager.Instance.NewCutList();
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

        private void GenerateExcel(DataTable orderListDataTable, DataTable cutListDataTable, string filePath) {
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            Excel.Worksheet workSheet1;
            Excel.Range cellRange;
            Excel.Range cellRange1;
            try {
                Excel.Application excel = new Excel.Application();
                excel.DisplayAlerts = false;
                excel.Visible = false;
                workBook = excel.Workbooks.Add(Type.Missing);

                workSheet1 = (Excel.Worksheet)workBook.ActiveSheet;
                workSheet1.Name = "Cut List";
                DataTable tempDt = cutListDataTable;
                workSheet1.Cells.Font.Size = 11;
                int rowcount = 1;
                for (int i = 2; i <= tempDt.Columns.Count; i++) //taking care of Headers.  
                {
                    if (i - 1 == 1 || i - 1 == 2 || i - 1 == tempDt.Columns.Count - 1) continue;
                    workSheet1.Cells[1, i - 3] = tempDt.Columns[i - 1].ColumnName;
                }
                foreach (DataRow row in tempDt.Rows) //taking care of each Row  
                    {
                    rowcount += 1;
                    for (int i = 1; i < tempDt.Columns.Count; i++) //taking care of each column  
                    {
                        if (i == 1 || i == 2 || i == tempDt.Columns.Count - 1) continue;
                        workSheet1.Cells[rowcount, i - 2] = row[i].ToString();
                    }
                }
                cellRange1 = workSheet1.Range[workSheet1.Cells[1, 1], workSheet1.Cells[rowcount, tempDt.Columns.Count]];
                cellRange1.EntireColumn.AutoFit();

                workSheet = (Excel.Worksheet)workBook.Worksheets.Add();
                workSheet.Name = "Order List";
                tempDt = orderListDataTable;
                workSheet.Cells.Font.Size = 11;
                rowcount = 1;
                for (int i = 2; i <= tempDt.Columns.Count; i++) //taking care of Headers.  
                {
                    if (i - 1 == 1 || i - 1 == 2) continue;
                    workSheet.Cells[1, i - 3] = tempDt.Columns[i - 1].ColumnName;
                }
                foreach (DataRow row in tempDt.Rows) //taking care of each Row  
                    {
                    rowcount += 1;
                    for (int i = 1; i < tempDt.Columns.Count; i++) //taking care of each column  
                     {
                        if (i == 1 || i == 2) continue;
                        workSheet.Cells[rowcount, i - 2] = row[i].ToString();
                    }
                }
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[rowcount, tempDt.Columns.Count]];
                cellRange.EntireColumn.AutoFit();



                workBook.SaveAs(filePath, Excel.XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
            } catch (Exception) {
                throw;
            }
        }

        private void GenerateCSV(DataTable orderListDataTable, DataTable cutListDataTable, string filePath) {
            FileStream fs = null;
            try {
                fs = new FileStream(filePath, FileMode.Create);
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) {
                    foreach (DataColumn col in orderListDataTable.Columns) {
                        if (col == orderListDataTable.Columns[0] || col == orderListDataTable.Columns[1] || col == orderListDataTable.Columns[2]) {
                            continue;
                        }
                        writer.Write(col.ColumnName + ",");
                    }
                    writer.Write("\n");
                    foreach (DataRow row in orderListDataTable.Rows) {
                        for (int i = 3; i < row.ItemArray.Length; i++) {
                            if (row.ItemArray[i].GetType() == typeof(string)) {
                                writer.Write("\"" + row.ItemArray[i] + "\",");
                            } else {
                                writer.Write(row.ItemArray[i].ToString() + ",");
                            }
                        }
                        writer.Write("\n");
                    }
                    foreach (DataColumn col in cutListDataTable.Columns) {
                        if (col == cutListDataTable.Columns[0] || col == cutListDataTable.Columns[1] || col == cutListDataTable.Columns[2] || col == cutListDataTable.Columns[cutListDataTable.Columns.Count - 1]) {
                            continue;
                        }
                        writer.Write(col.ColumnName + ",");
                    }
                    writer.Write("\n");
                    foreach (DataRow row in cutListDataTable.Rows) {
                        for (int i = 3; i < row.ItemArray.Length - 1; i++) {
                            if (row.ItemArray[i].GetType() == typeof(string)) {
                                writer.Write("\"" + row.ItemArray[i] + "\",");
                            } else {
                                writer.Write(row.ItemArray[i].ToString() + ",");
                            }
                        }
                        writer.Write("\n");
                    }
                }
            } catch (Exception) {
                MessageWindow.Show("Save error", "Unable to save file, try again.", MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                if (fs != null) {
                    fs.Dispose();
                }
            }
        }

        #endregion
    }
}
