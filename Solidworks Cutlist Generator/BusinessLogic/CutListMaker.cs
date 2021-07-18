using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Solidworks_Cutlist_Generator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Solidworks_Cutlist_Generator.BusinessLogic {
    class CutListMaker {
        public SldWorks swApp;
        int fileerror = 0;
        int filewarning = 0;
        bool inBodyFolder = false;
        public ObservableCollection<CutItem> CutList { get; set; }

        public CutListMaker() {
            CutList = new ObservableCollection<CutItem>();
        }
        internal void NewCutList() {
            CutList = new ObservableCollection<CutItem>();
        }

        public ObservableCollection<CutItem> Generate(string filePath, bool isDetailed) {
            bool isPart;
            bool isAssembly;

            inBodyFolder = false;


            isPart = filePath.ToLower().Contains(".sldprt");
            isAssembly = filePath.ToLower().Contains(".sldasm");

            if (!isPart && !isAssembly) {
                string messageBoxText = "You must select an assembly or part file!";
                string caption = "DERRRRP!";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return null;
            }
            var progId = "SldWorks.Application";
            var progType = Type.GetTypeFromProgID(progId);
            swApp = new SldWorks();
            swApp.Visible = true;
            // increase performance 
            ModelDoc2 doc;

            doc = isAssembly ?
                swApp.OpenDoc6(filePath, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning) :
                swApp.OpenDoc6(filePath, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning);

            // Set the working directory to the document directory
            swApp.SetCurrentWorkingDirectory(doc.GetPathName().Substring(0, doc.GetPathName().LastIndexOf("\\")));

            swApp.CommandInProgress = true;
            ModelDoc2 swModel;
            AssemblyDoc swAssy = default(AssemblyDoc);
            PartDoc swPart = default(PartDoc);
            swModel = swApp.ActiveDoc as ModelDoc2;
            if (isAssembly) {
                swAssy = (AssemblyDoc)swModel;
            } else {
                swPart = (PartDoc)swModel;
            }

            if (isAssembly) {
                TraverseAssembly(swAssy);
            } else {
                TraverseFeatures((Feature)swPart.FirstFeature(), true, "Root Feature");
            }
            while (swApp.ActiveDoc != null) {
                swApp.CloseDoc(((ModelDoc2)swApp.ActiveDoc).GetPathName());
            }
            if (!isDetailed) {
                Consolidate(CutList);
                return CutList;
            }
            SortCuts();
            return CutList;

        }

        public void SortCuts() {
            ObservableCollection<CutItem> temp = new ObservableCollection<CutItem>();
            StockItem sType = null;
            float leftOnStick = 0;
            int stickNum = 1;

            foreach (CutItem item in CutList) {
                for (int i = 0; i < item.Qty; i++) {
                    temp.Add(item.Clone());
                }
            }
            CutList = temp;
            temp = new ObservableCollection<CutItem>();
            CutList.Sort();
            CutList.Reverse();
            int count = CutList.Count;

            for (int i = count - 1; i > -1; i--) {
                if (i == count - 1) {
                    sType = CutList[i].StockType;
                    CutList[i].StickNumber = stickNum;
                    leftOnStick = CutList[i].StockType.StockLengthInInches - CutList[i].Length;
                    temp.Add(CutList[i]);
                    CutList.RemoveAt(i);
                } else if (CutList[i].StockType == sType) {
                    if (leftOnStick >= CutList[i].Length) {
                        leftOnStick -= CutList[i].Length;
                        CutList[i].StickNumber = stickNum;
                        temp.Add(CutList[i]);
                        CutList.RemoveAt(i);
                    } else {
                        bool isBroken = false;
                        bool areMoreOfStockType = false;
                        for (int j = i - 1; j > -1; j--) {
                            if (CutList[j].StockType == sType) {
                                areMoreOfStockType = true;
                                if (leftOnStick >= CutList[j].Length) {
                                    leftOnStick -= CutList[j].Length;
                                    CutList[j].StickNumber = stickNum;
                                    temp.Add(CutList[j]);
                                    CutList.RemoveAt(j);
                                    isBroken = true;
                                    break;
                                }
                            }
                        }
                        if (!isBroken && areMoreOfStockType) {
                            leftOnStick = CutList[i].StockType.StockLengthInInches;
                            stickNum++;
                            i++;
                            continue;
                        } else if (isBroken) {
                            i++;
                            continue;
                        } else {
                            stickNum++;
                            CutList[i].StickNumber = stickNum;
                            temp.Add(CutList[i]);
                            CutList.RemoveAt(i);
                            stickNum = 1;
                            if (i > 0) {
                                leftOnStick = CutList[i - 1].StockType.StockLengthInInches;
                                sType = CutList[i - 1].StockType;
                            }
                            continue;
                        }
                    }
                } else {
                    sType = CutList[i].StockType;
                    stickNum = 1;
                    leftOnStick = CutList[i].StockType.StockLengthInInches - CutList[i].Length;
                    CutList[i].StickNumber = stickNum;
                    temp.Add(CutList[i]);
                    CutList.RemoveAt(i);
                }
            }
            Consolidate(temp);
            CutList = temp;
        }

        public void Consolidate(ObservableCollection<CutItem> cList) {
            for (int i = cList.Count - 1; i > -1; i--) {
                for (int j = i - 1; j > -1; j--) {
                    if (cList[i].Description == cList[j].Description &&
                        cList[i].Length == cList[j].Length &&
                        cList[i].Angle1 == cList[j].Angle1 &&
                        cList[i].Angle2 == cList[j].Angle2 &&
                        cList[i].StickNumber == cList[j].StickNumber) {
                        cList[j].Qty += cList[i].Qty;
                        cList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void AddCutItem(Feature thisFeat) {
            CustomPropertyManager CustomPropMgr = default(CustomPropertyManager);
            CustomPropMgr = (CustomPropertyManager)thisFeat.CustomPropertyManager;
            string[] vCustomPropNames;
            vCustomPropNames = (string[])CustomPropMgr.GetNames();
            if ((vCustomPropNames != null)) {
                int i = 0;
                int qty = 0;
                float length = 0;
                float angle1 = 0;
                float angle2 = 0;
                string angleDirection = "";
                string angleRotation = "";
                string description = "";
                string material = "";
                bool isNew = false;
                StockItem sItem = null;

                using (var ctx = new CutListGeneratorContext()) {
                    for (i = 0; i <= (vCustomPropNames.Length - 1); i++) {
                        string CustomPropName = (string)vCustomPropNames[i];
                        string CustomPropResolvedVal;
                        CustomPropMgr.Get2(CustomPropName, out _, out CustomPropResolvedVal);
                        switch (CustomPropName.ToLower()) {
                            case "quantity":
                                Int32.TryParse(CustomPropResolvedVal, out qty);
                                break;
                            case "description":
                                description = CustomPropResolvedVal;
                                sItem = ctx.StockItems.Where(item => item.Description == description).FirstOrDefault(); ;
                                if (sItem == null) {
                                    isNew = true;
                                }
                                break;
                            case "length":
                                float.TryParse(CustomPropResolvedVal, out length);
                                break;
                            case "angle1":
                                float.TryParse(CustomPropResolvedVal.Substring(0, CustomPropResolvedVal.Length - 1), out angle1);
                                break;
                            case "angle2":
                                float.TryParse(CustomPropResolvedVal.Substring(0, CustomPropResolvedVal.Length - 1), out angle2);
                                break;
                            case "material":
                                material = CustomPropResolvedVal;
                                break;
                            case "angle direction":
                                angleDirection = CustomPropResolvedVal;
                                break;
                            case "angle rotation":
                                angleRotation = CustomPropResolvedVal;
                                break;
                            default:
                                break;
                        }
                    }
                    if (isNew) {
                        sItem = new StockItem(description: description,
                            materialType: StockItem.MaterialFromDescription(description),
                            profType: StockItem.ProfileFromDescription(description));
                        ctx.StockItems.Add(sItem);
                        ctx.SaveChanges();
                    }
                    isNew = false;

                    CutItem cItem = new CutItem(sItem, qty, length, angle1, angle2, angleDirection, angleRotation);
                    CutList.Add(cItem);
                }
            }
        }

        public ObservableCollection<CutItem> FindCutlist(Feature thisFeat, string ParentName) {
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
                return null;
            }

            //Only consider the SubWeldFolder that are under the SolidBodyFolder
            if ((inBodyFolder == false) & (FeatType == "SubWeldFolder")) {
                //Skip the second occurrence of the SubWeldFolders during the feature traversal
                return null;
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
                    return null;
                }
            }

            if (FeatType == "CutListFolder") {
                if (BodyCount > 0) {
                    AddCutItem(thisFeat);
                }
            }
            return null;
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
                action(feature, true, "Root Feature");
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
                ObservableCollection<CutItem> temp = FindCutlist(curFeat, ParentName);
                if (temp != null) {
                    foreach (CutItem item in temp) {
                        CutList.Add(item);
                    }
                }
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
            }
        }
    }
}
