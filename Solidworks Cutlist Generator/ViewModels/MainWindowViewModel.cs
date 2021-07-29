using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Solidworks_Cutlist_Generator.Model;
using Solidworks_Cutlist_Generator.BusinessLogic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using Solidworks_Cutlist_Generator.ViewModels.Commands;

namespace Solidworks_Cutlist_Generator.ViewModels {
    class MainWindowViewModel : ViewModelBase {
        private readonly object asyncLock;
        private string sourceText;
        private string loadingText;
        private bool isDetailed;
        private bool showPricing;
        private bool isLoading;
        private ObservableCollection<Vendor> vendors;
        private ObservableCollection<StockItem> stockItems;
        private ObservableCollection<CutItem> cutList;
        private DataTable orderList;

        public RelayCommand GenerateCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand ClearCommand { get; set; }

        public RelayCommand SourceBrowseCommand { get; set; }

        public CutListMaker CutListMaker { get; set; }

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

        public DataTable OrderList {
            get => orderList;
            set {
                SetProperty(ref orderList, value);
                BindingOperations.EnableCollectionSynchronization(orderList.AsEnumerable(), asyncLock);
            }
        }

        public MainWindowViewModel() {
            GenerateCommand = new RelayCommand(() => GenerateCutList() , () => !string.IsNullOrEmpty(SourceText));
            SaveCommand = new RelayCommand(() => SaveCutList(), () => CutList != null && CutList.Count > 0);
            ClearCommand = new RelayCommand(() => ClearCutList(), () => CutList != null && CutList.Count > 0);
            SourceBrowseCommand = new RelayCommand(() => SourceBrowse());
            asyncLock = new object();
            CutList = new ObservableCollection<CutItem>();
            OrderList = new DataTable();
            CutListMaker = new CutListMaker();
            Vendors = CutListMaker.Vendors;
            StockItems = CutListMaker.StockItems;
            LoadingText = "Loading...";
            IsLoading = false;
        }

        #region Button Clicks

        public async void GenerateCutList() {
            IsLoading = true;
            await Task.Run(() => CutListMaker.Generate(SourceText, IsDetailed));
            CutListMaker.RefreshGrids();
            IsLoading = false;
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
                    GenerateExcel(ToDataTable(CutListMaker.CutList), filePath);
                } else if (filePath.Contains(".csv")) {
                    GenerateCSV(ToDataTable(CutListMaker.CutList), filePath);
                }
            }
        }

        public void ClearCutList() {
            CutListMaker.NewCutList();
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
