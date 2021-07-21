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

namespace Solidworks_Cutlist_Generator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window {

        Excel.Workbook workBook;
        Excel.Worksheet workSheet;
        Microsoft.Office.Interop.Excel.Range cellRange;
        CutListMaker CutListMaker;

        public MainWindow() {
            InitializeComponent();
            CutListMaker = new CutListMaker();
            cutListDataGrid.ItemsSource = CutListMaker.CutList;
            RefreshGrids();
        }

        private void RefreshGrids() {
            using (var ctx = new CutListGeneratorContext()) {
                var sTypes = from i in ctx.StockItems
                             select i;
                stockTypesDataGrid.ItemsSource = sTypes.ToList();
                var vendors = from i in ctx.Vendors
                              select i;
                vendorDataGrid.ItemsSource = vendors.ToList();
                //    var angle = StockItem.CreateStockItem(description: "angle");
                //    ctx.StockItems.Add(angle);
                //    ctx.SaveChanges();
            }
        }

        #region Button Clicks

        private async void generateButton_Click(object sender, RoutedEventArgs e) {
            string filePath = filePathTextBox.Text;
            bool isDetailed = (bool)detailedCutListCheckBox.IsChecked;
            cutListDataGrid.ItemsSource = null;
            var result = Task.Run(() => CutListMaker.Generate(filePath, isDetailed));
            cutListDataGrid.ItemsSource = await result;
            RefreshGrids();
        }

        private void sourceBrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
                filePathTextBox.Text = openFileDialog.FileName;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
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

        private void clearButton_Click(object sender, RoutedEventArgs e) {
            cutListDataGrid.ItemsSource = null;
            CutListMaker.NewCutList();
            cutListDataGrid.ItemsSource = CutListMaker.CutList;
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
