using Microsoft.Win32;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Solidworks_Cutlist_Generator.Model;
using Solidworks_Cutlist_Generator.BusinessLogic;

namespace Solidworks_Cutlist_Generator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {


        List<CutItem> CutList = new List<CutItem>();
        CutListMaker CutListMaker;

        public MainWindow() {
            InitializeComponent();
            CutListMaker = new CutListMaker(CutList, filePathTextBox);
            using (var ctx = new CutListGeneratorContext()) {
                //var angle = StockItem.CreateStockItem(description: "angle");

                //ctx.StockItems.Add(angle);
                //ctx.SaveChanges();
            }
        }

        #region Button Clicks

        private void generateButton_Click(object sender, RoutedEventArgs e) {
            CutListMaker.Generate();
        }

        private void sourceBrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                filePathTextBox.Text = openFileDialog.FileName;
        }

        private void outputBrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                outputPathTextBox.Text = openFileDialog.FileName;
        }

        private void outputBrowseButton_Click_1(object sender, RoutedEventArgs e) {

        }

        #endregion

    }
}
