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
        public SldWorks swApp;
        int fileerror = 0;
        int filewarning = 0;
        bool inBodyFolder = false;

        Dictionary<string, int> modelDict = new Dictionary<string, int>();
        List<CutItem> CutList = new List<CutItem>();


        public MainWindow() {
            InitializeComponent();
            using (var ctx = new CutlistGeneratorContext()) {
                //var angle = StockItem.CreateStockItem(description: "angle");

                //ctx.StockItems.Add(angle);
                //ctx.SaveChanges();
            }
        }

        #region Button Clicks

        private void generateButton_Click(object sender, RoutedEventArgs e) {
            Generate();
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

        #region Business Logic
        private void Generate() {
            bool isPart;
            bool isAssembly;

            modelDict.Clear();
            inBodyFolder = false;


            isPart = filePathTextBox.Text.ToLower().Contains(".sldprt");
            isAssembly = filePathTextBox.Text.ToLower().Contains(".sldasm");

            if (!isPart && !isAssembly) {
                string messageBoxText = "You must select an assembly or part file!";
                string caption = "DERRRRP!";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return;
            }
            var progId = "SldWorks.Application";
            var progType = System.Type.GetTypeFromProgID(progId);
            swApp = new SldWorks();
            swApp.Visible = true;
            // increase performance 
            ModelDoc2 doc;

            doc = isAssembly ?
                swApp.OpenDoc6(filePathTextBox.Text, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning) :
                swApp.OpenDoc6(filePathTextBox.Text, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning);

            // Set the working directory to the document directory
            swApp.SetCurrentWorkingDirectory(doc.GetPathName().Substring(0, doc.GetPathName().LastIndexOf("\\")));

            swApp.CommandInProgress = true;
            ModelDoc2 swModel;
            AssemblyDoc swAssy = default(AssemblyDoc);
            PartDoc swPart = default(PartDoc);
            string fileName;
            string tmpPath;
            swModel = swApp.ActiveDoc as ModelDoc2;
            if (isAssembly) {
                swAssy = (AssemblyDoc)swModel;
            } else {
                swPart = (PartDoc)swModel;
            }
            tmpPath = swModel.GetPathName();
            string[] tok;
            tok = tmpPath.Split('\\');

            // reconstruct the assembly path without the file name
            int i;
            string virAssPath = "";
            for (i = 0; i < tok.Length - 1; i++) {
                virAssPath = virAssPath + tok[i] + "\\";
            }
            fileName = tok[tok.Length - 1] + "_Cutlist.XXX";
            Debug.Print(fileName);

            if (isAssembly) {
                TraverseAssembly(swAssy);
            } else {
                TraverseFeatures((Feature)swPart.FirstFeature(), true, "Root Feature");
            }
            while (swApp.ActiveDoc != null) {
                swApp.CloseDoc(((ModelDoc2)swApp.ActiveDoc).GetPathName());
            }
        }

        public void GetFeatureCustomProps(Feature thisFeat) {
            CustomPropertyManager CustomPropMgr = default(CustomPropertyManager);
            CustomPropMgr = (CustomPropertyManager)thisFeat.CustomPropertyManager;
            string[] vCustomPropNames;
            vCustomPropNames = (string[])CustomPropMgr.GetNames();
            if ((vCustomPropNames != null)) {
                Debug.Print("\nCut-list custom properties:");
                int i = 0;
                int qty = 0;
                float length = 0;
                float angle1 = 0;
                float angle2 = 0;
                string description = "";
                string material = "";
                bool isNew = false;
                StockItem sItem = null;



                //ctx.StockItems.Add(angle);
                //ctx.SaveChanges();
                using (var ctx = new CutlistGeneratorContext()) {
                    for (i = 0; i <= (vCustomPropNames.Length - 1); i++) {
                        string CustomPropName = (string)vCustomPropNames[i];
                        string CustomPropResolvedVal;
                        CustomPropMgr.Get2(CustomPropName, out _, out CustomPropResolvedVal);
                        switch (CustomPropName) {
                            case "QUANTITY":
                                Int32.TryParse(CustomPropResolvedVal, out qty);
                                break;
                            case "Description":
                                description = CustomPropResolvedVal;
                                sItem = ctx.StockItems.Where(item => item.Description == description).FirstOrDefault(); ;
                                if (sItem == null) {
                                    isNew = true;
                                }
                                break;
                            case "LENGTH":
                                float.TryParse(CustomPropResolvedVal, out length);
                                break;
                            case "ANGLE1":
                                float.TryParse(CustomPropResolvedVal, out angle1);
                                break;
                            case "ANGLE2":
                                float.TryParse(CustomPropResolvedVal, out angle2);
                                break;
                            case "MATERIAL":
                                material = CustomPropResolvedVal;
                                break;
                            default:
                                break;
                        }
                        Debug.Print("Name: " + CustomPropName);
                        Debug.Print("Resolved value: " + CustomPropResolvedVal);
                    }
                    if (isNew) {
                        sItem = new StockItem(description: description,
                            materialType: StockItem.MaterialFromDescription(description),
                            profType: StockItem.ProfileFromDescription(description));
                        ctx.StockItems.Add(sItem);
                        ctx.SaveChanges();
                        isNew = false;
                    }

                    CutItem cItem = new CutItem(sItem, qty, length, angle1, angle2);
                    CutList.Add(cItem);
                }


                //Check if StockItem is already in db, if not create.
                //add cuts to temporary list. in db?

            }
        }

        public void FindCutlist(Feature thisFeat, string ParentName) {

            int BodyCount = 0;

            string FeatType = null;
            FeatType = thisFeat.GetTypeName();
            if ((FeatType == "SolidBodyFolder") & (ParentName == "Root Feature")) {
                inBodyFolder = true;
            }
            if ((FeatType != "SolidBodyFolder") & (ParentName == "Root Feature")) {
                inBodyFolder = false;
            }

            //Only consider the CutListFolders that are under SolidBodyFolder
            if ((inBodyFolder == false) & (FeatType == "CutListFolder")) {
                return;
            }

            //Only consider the SubWeldFolder that are under the SolidBodyFolder
            if ((inBodyFolder == false) & (FeatType == "SubWeldFolder")) {
                //Skip the second occurrence of the SubWeldFolders during the feature traversal
                return;
            }

            bool IsBodyFolder;
            if (FeatType == "SolidBodyFolder" | FeatType == "SurfaceBodyFolder" | FeatType == "CutListFolder" | FeatType == "SubWeldFolder" | FeatType == "SubAtomFolder") {
                IsBodyFolder = true;
            } else {
                IsBodyFolder = false;
            }

            if (IsBodyFolder) {
                BodyFolder BodyFolder = default(BodyFolder);
                BodyFolder = (BodyFolder)thisFeat.GetSpecificFeature2();
                BodyCount = BodyFolder.GetBodyCount();
                if ((FeatType == "CutListFolder") & (BodyCount < 1)) {
                    //When BodyCount = 0, this cut list folder is not displayed in the
                    //FeatureManager design tree, so skip it
                    return;
                }
            }

            if (FeatType == "CutListFolder") {
                if (BodyCount > 0) {
                    GetFeatureCustomProps(thisFeat);
                }
            }
        }

        public void TraverseComponent(Component2 swComp, Action<Feature, bool, string> action) {
            object[] vChildComp;
            Component2 swChildComp;

            vChildComp = (object[])swComp.GetChildren();
            if (vChildComp.Length > 0) {
                for (int i = 0; i < vChildComp.Length; i++) {
                    swChildComp = (Component2)vChildComp[i];
                    TraverseComponent(swChildComp, action);
                }
            } else {

                ModelDoc2 model = swApp.OpenDoc6(swComp.GetPathName(), (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning); ;
                Feature feature = (Feature)model.FirstFeature();
                if (modelDict.ContainsKey(swComp.GetPathName())) {
                    modelDict[swComp.GetPathName()]++;
                    Debug.Print(swComp.Name.Substring(0, swComp.Name.IndexOf("-")) + ": " + modelDict[swComp.GetPathName()]);
                } else {
                    modelDict[swComp.GetPathName()] = 1;
                    Debug.Print("\n*********\n" + swComp.Name.Substring(0, swComp.Name.IndexOf("-")) + "\n*********\n");
                    action(feature, true, "Root Feature");
                }
            }

        }

        public void TraverseAssembly(AssemblyDoc swAssy) {
            object[] vChildComp;
            Component2 swChildComp;

            vChildComp = (object[])swAssy.GetComponents(false);
            for (int i = 0; i < vChildComp.Length; i++) {
                swChildComp = (Component2)vChildComp[i];
                ModelDoc2 model = (ModelDoc2)swChildComp.GetModelDoc2();
                Feature feat = (Feature)model.FirstFeature();
                TraverseComponent(swChildComp, TraverseFeatures);
            }
        }

        public void TraverseFeatures(Feature thisFeat, bool isTopLevel, string ParentName) {
            Feature curFeat = default(Feature);
            curFeat = thisFeat;
            while ((curFeat != null)) {
                FindCutlist(curFeat, ParentName);
                Feature subfeat = default(Feature);
                subfeat = (Feature)curFeat.GetFirstSubFeature();
                while ((subfeat != null)) {
                    TraverseFeatures(subfeat, false, curFeat.Name);
                    Feature nextSubFeat = default(Feature);
                    nextSubFeat = (Feature)subfeat.GetNextSubFeature();
                    subfeat = (Feature)nextSubFeat;
                    nextSubFeat = null;
                }
                subfeat = null;
                Feature nextFeat = default(Feature);
                if (isTopLevel) {
                    nextFeat = (Feature)curFeat.GetNextFeature();
                } else {
                    nextFeat = null;
                }
                curFeat = (Feature)nextFeat;
                nextFeat = null;
            }
        }
        #endregion

    }
}
