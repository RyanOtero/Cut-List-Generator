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

namespace Solidworks_Cutlist_Generator.ViewModels {
    class MainWindowViewModel : ViewModelBase {
        private readonly object cutListLock;
        private string sourceText;
        private bool isDetailed;
        private ObservableCollection<Vendor> vendors;
        private ObservableCollection<StockItem> stockItems;
        private ObservableCollection<CutItem> cutList;

        public GenerateCommand GenerateCommand { get; set; }

        public SaveCommand SaveCommand { get; set; }

        public ClearCommand ClearCommand { get; set; }

        public SourceBrowseCommand SourceBrowseCommand { get; set; }

        public CutListMaker CutListMaker { get; set; }

        public bool IsDetailed {
            get => isDetailed;
            set { SetProperty(ref isDetailed, value); }
        }

        public string SourceText {
            get => sourceText;
            set { SetProperty(ref sourceText, value); }
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
            get => cutList; set {
                SetProperty(ref cutList, value);
                BindingOperations.EnableCollectionSynchronization(cutList, cutListLock);
            }
        }

        public MainWindowViewModel() {
            GenerateCommand = new GenerateCommand(this);
            SaveCommand = new SaveCommand(this);
            ClearCommand = new ClearCommand(this);
            SourceBrowseCommand = new SourceBrowseCommand(this);
            cutListLock = new object();
            CutList = new ObservableCollection<CutItem>();
            CutListMaker = new CutListMaker(CutList);
            Vendors = new ObservableCollection<Vendor>();
            StockItems = new ObservableCollection<StockItem>();
            RefreshGrids();
        }

        private void RefreshGrids() {
            using (var ctx = new CutListGeneratorContext()) {
                var sTypes = from i in ctx.StockItems
                             select i;
                foreach (StockItem item in sTypes.ToList()) {
                    StockItems.Add(item);
                }
                var vendors = from i in ctx.Vendors
                              select i;
                foreach (Vendor item in vendors.ToList()) {
                    Vendors.Add(item);
                }
                //    var angle = StockItem.CreateStockItem(description: "angle");
                //    ctx.StockItems.Add(angle);
                //    ctx.SaveChanges();
            }
        }

        #region Button Clicks

        public async void GenerateCutList() {
            await Task.Run(() => CutListMaker.Generate(SourceText, IsDetailed));
            RefreshGrids();
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
            //cutListDataGrid.ItemsSource = null;
            CutListMaker.NewCutList();
            //cutListDataGrid.ItemsSource = CutListMaker.CutList;
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
