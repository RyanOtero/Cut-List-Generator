using Microsoft.EntityFrameworkCore;
using SolidPrice.Utils;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static SolidPrice.Utils.Messenger;

namespace SolidPrice.Models {
    public class CutListManager : INotifyPropertyChanged {

        #region Fields
        private static readonly CutListManager instance = new CutListManager();
        int fileerror = 0;
        int filewarning = 0;
        bool inBodyFolder = false;
        private ObservableCollection<CutItem> cutList;
        private readonly object asyncLock;
        private ObservableCollection<OrderItem> orderList;
        private ObservableCollection<Vendor> vendors;
        private ObservableCollection<StockItem> stockItems;
        private ObservableCollection<SheetCutItem> sheetCutList;
        private ObservableCollection<SheetOrderItem> sheetOrderList;
        private ObservableCollection<SheetStockItem> sheetStockItems;
        private string connectionString;
        private bool hadError;
        private CutListGeneratorContext ctx;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties

        public static CutListManager Instance => instance;

        public SldWorks SWApp { get; set; }

        public string ConnectionString {
            get => connectionString;
            set { connectionString = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CutItem> CutList {
            get => cutList;
            set {
                cutList = value;
                BindingOperations.EnableCollectionSynchronization(cutList, asyncLock);
            }
        }

        public ObservableCollection<OrderItem> OrderList {
            get => orderList;
            set {
                orderList = value;
                BindingOperations.EnableCollectionSynchronization(orderList, asyncLock);
            }
        }

        public ObservableCollection<StockItem> StockItems {
            get => stockItems;
            set {
                stockItems = value;
                BindingOperations.EnableCollectionSynchronization(stockItems, asyncLock);
            }
        }

        public ObservableCollection<SheetCutItem> SheetCutList {
            get => sheetCutList;
            set {
                sheetCutList = value;
                BindingOperations.EnableCollectionSynchronization(sheetCutList, asyncLock);
            }
        }

        public ObservableCollection<SheetOrderItem> SheetOrderList {
            get => sheetOrderList;
            set {
                sheetOrderList = value;
                BindingOperations.EnableCollectionSynchronization(sheetOrderList, asyncLock);
            }
        }

        public ObservableCollection<SheetStockItem> SheetStockItems {
            get => sheetStockItems;
            set {
                sheetStockItems = value;
                BindingOperations.EnableCollectionSynchronization(sheetStockItems, asyncLock);
            }
        }

        public ObservableCollection<Vendor> Vendors {
            get => vendors;
            set {
                vendors = value;
                BindingOperations.EnableCollectionSynchronization(vendors, asyncLock);
            }
        }

        #endregion

        #region Constructors

        private CutListManager() {
            asyncLock = new object();
            CutList = new ObservableCollection<CutItem>();
            StockItems = new ObservableCollection<StockItem>();
            OrderList = new ObservableCollection<OrderItem>();
            Vendors = new ObservableCollection<Vendor>();
            SheetCutList = new ObservableCollection<SheetCutItem>();
            SheetStockItems = new ObservableCollection<SheetStockItem>();
            SheetOrderList = new ObservableCollection<SheetOrderItem>();
            Vendors = new ObservableCollection<Vendor>();

            Task.Run(() => {
                try {
                    var progId = "SldWorks.Application";
                    var progType = System.Type.GetTypeFromProgID(progId);
                    SWApp = Activator.CreateInstance(progType) as SldWorks;
                    SWApp.Visible = false;
                } catch (Exception e) {
                    ErrorMessage("SolidWorks Error", "Unable to connect to SolidWorks. Please make sure that SolidWorks is installed in the default location.");
                }
            });
        }
        #endregion


        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Other Methods
        public void Refresh() {
            if (!bool.Parse(App.Current.Properties["IsCreated"].ToString())) {
                using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {
                    return;
                }
            }
            if (ConnectionString.Contains("server=;") || ConnectionString.Contains("database=;")
                || ConnectionString.Contains("user=;") || ConnectionString.Contains("password=;")) {
                ErrorMessage("Database Error clm.cs 110", "The connection string is incomplete.");
                return;
            }
            if (string.IsNullOrEmpty(ConnectionString)) {
                ErrorMessage("Database Error clm.cs 115", "The connection string is invalid.");
                return;
            }
            CutList.Clear();
            OrderList.Clear();
            StockItems.Clear();
            SheetCutList.Clear();
            SheetOrderList.Clear();
            SheetStockItems.Clear();
            Vendors.Clear();
            try {
                using (var ctx = new CutListGeneratorContext(ConnectionString)) {
                    List<Vendor> vendors = ctx.Vendors.ToList();
                    foreach (Vendor item in vendors) {
                        Vendors.Add(item);
                    }

                    List<StockItem> sTypes = ctx.StockItems.Include(s => s.Vendor).ToList();
                    foreach (StockItem item in sTypes) {
                        StockItems.Add(item);
                    }

                    List<SheetStockItem> sSTypes = ctx.SheetStockItems.Include(s => s.Vendor).ToList();
                    foreach (SheetStockItem item in sSTypes) {
                        SheetStockItems.Add(item);
                    }

                    List<CutItem> cList = ctx.CutItems.Include(c => c.StockItem).ToList();
                    cList.Sort();
                    foreach (CutItem item in cList) {
                        CutList.Add(item);
                    }

                    List<SheetCutItem> sCList = ctx.SheetCutItems.Include(c => c.SheetStockItem).ToList();
                    sCList.Sort();
                    foreach (SheetCutItem item in sCList) {
                        SheetCutList.Add(item);
                    }

                    List<OrderItem> oList = ctx.OrderItems.Include(c => c.StockItem).ToList();
                    oList.Sort();
                    foreach (OrderItem item in oList) {
                        OrderList.Add(item);
                    }

                    List<SheetOrderItem> sOList = ctx.SheetOrderItems.Include(c => c.SheetStockItem).ToList();
                    sOList.Sort();
                    foreach (SheetOrderItem item in sOList) {
                        SheetOrderList.Add(item);
                    }




                }
            } catch (Exception e) {
                ErrorMessage("Database Error clm.cs 134", "There was an error while accessing the database.");
            }
        }

        public void NewCutList() {
            try {
                using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {
                    ctx.CutItems.RemoveRange(ctx.CutItems);
                    ctx.OrderItems.RemoveRange(ctx.OrderItems);
                    ctx.SheetCutItems.RemoveRange(ctx.SheetCutItems);
                    ctx.SheetOrderItems.RemoveRange(ctx.SheetOrderItems);
                    ctx.SaveChanges();
                }
            } catch (Exception e) {
                ErrorMessage("Database Error clm.cs 148", "There was an error while accessing the database.");

            }
            CutList.Clear();
            OrderList.Clear();
            SheetOrderList.Clear();
            sheetCutList.Clear();
        }
        #endregion

        #region Generation
        public void Generate(string filePath, bool isDetailed) {
            if (SWApp == null) {
                ErrorMessage("SolidWorks Error", "Unable to connect to SolidWorks.");
                return;
            }
            Stopwatch timer = new Stopwatch();
            timer.Start();
            if (string.IsNullOrEmpty(filePath)) return;

            bool isPart;
            bool isAssembly;

            inBodyFolder = false;
            List<CutItem> tempCList = new List<CutItem>();
            List<SheetCutItem> tempSCList = new List<SheetCutItem>();
            isPart = filePath.ToLower().Contains(".sldprt");
            isAssembly = filePath.ToLower().Contains(".sldasm");

            if (!isPart && !isAssembly) {
                ErrorMessage("Invalid File clm.cs 173", "Please select a part file or an assembly file.");
                return;
            }

            List<string> openFiles = new();
            ModelDoc2 doc;
            doc = (ModelDoc2)SWApp.GetFirstDocument();
            while (doc != null) {
                openFiles.Add(doc.GetPathName());
                doc = (ModelDoc2)doc.GetNext();
            }
            MessageBoxResult result;
            if (openFiles.Count != 0 || openFiles.Count >= 1 && openFiles[0] != filePath) {
                result = YesNoMessage("Solidworks", "All open Solidworks documents will be closed without saving. Proceed?");
                if (result != MessageBoxResult.Yes) {
                    return;
                } else {
                    for (int i = openFiles.Count - 1; i > -1; i--) {
                        if (openFiles[i] != filePath) {
                            SWApp.CloseDoc(openFiles[i]);
                        }
                    }
                    doc = (ModelDoc2)SWApp.GetFirstDocument();
                }
            }

            if (doc == null) {
                try {
                    doc = isAssembly ?
                        SWApp.OpenDoc6(filePath, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning) :
                        SWApp.OpenDoc6(filePath, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref fileerror, ref filewarning);
                } catch (Exception e) {
                    ErrorMessage("SolidWorks Error", "There was an error while trying to access SolidWorks.");
                    return;
                }
            }



            // Set the working directory to the document directory
            SWApp.SetCurrentWorkingDirectory(doc.GetPathName().Substring(0, doc.GetPathName().LastIndexOf("\\")));

            SWApp.CommandInProgress = true;
            ModelDoc2 swModel;
            Component2 swAssy = default(Component2);
            PartDoc swPart = default(PartDoc);
            swModel = SWApp.ActiveDoc as ModelDoc2;
            ConfigurationManager swConfMgr = swModel.ConfigurationManager;
            Configuration swConf = swConfMgr.ActiveConfiguration;

            if (isAssembly) {
                swAssy = swConf.GetRootComponent() as Component2;
            } else {
                swPart = (PartDoc)swModel;
            }
            try {
                using (ctx = new CutListGeneratorContext(ConnectionString)) {
                    if (isAssembly) {
                        TraverseComponent(tempCList, tempSCList, swAssy, TraverseFeatures);
                    } else {
                        TraverseFeatures(tempCList, tempSCList, (Feature)swPart.FirstFeature(), true, "Root Feature", "");
                    }
                }

            } catch (Exception e) {

                //throw;
            }

            SWApp.CloseAllDocuments(true);

            if (hadError) {
                hadError = false;
                return;
            }
            SortCutListForDisplay(isDetailed, tempCList, tempSCList);

            timer.Stop();
            TimeSpan ts = timer.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Debug.Print("RunTime " + elapsedTime);
        }

        public void SortCutListForDisplay(bool isDetailed, List<CutItem> tempCList, List<SheetCutItem> tempSCList) {

            /////////// TO DO: Sort Cuts for sheet metal

            SortCuts(tempCList);
            List<CutItem> tempOrderList = new List<CutItem>();
            List<string> distinctOrderItems = new List<string>();
            for (int i = tempCList.Count - 1; i > -1; i--) {
                if (distinctOrderItems.Contains(tempCList[i].Description)) {
                    continue;
                }
                distinctOrderItems.Add(tempCList[i].Description);
                tempOrderList.Add(tempCList[i]);
            }

            var queryResult = from c in tempOrderList
                              join v in Vendors on c.StockItem.Vendor equals v into tol
                              from x in tol.DefaultIfEmpty()
                              select new OrderItem(c.StickNumber, c.StockItem);

            List<OrderItem> qList = queryResult.ToList();
            qList.Sort();
            foreach (OrderItem item in qList) {
                OrderList.Add(item);
            }

            if (!isDetailed) {
                foreach (CutItem item in tempCList) {
                    item.StickNumber = 0;
                }
                Consolidate(tempCList);
            }
            tempCList.Sort();
            foreach (CutItem item in tempCList) {
                CutList.Add(item);
            }

            try {
                using (CutListGeneratorContext ctx = new CutListGeneratorContext(ConnectionString)) {
                    ctx.CutItems.RemoveRange(ctx.CutItems);
                    ctx.OrderItems.RemoveRange(ctx.OrderItems);
                    foreach (CutItem item in CutList) {
                        CutItem c = item.Clone();
                        c.StockItem = null;
                        ctx.CutItems.Add(c);
                    }
                    foreach (OrderItem item in OrderList) {
                        OrderItem s = item.Clone();
                        s.StockItem = null;
                        ctx.OrderItems.Add(s);
                    }
                    ctx.SaveChanges();
                }
            } catch (Exception e) {
                throw;
            }
        }

        public static void BestStick(int sticknumber, float capacity, List<CutItem> cutItemsInput, List<CutItem> cutItemsOutput) {
            List<CutItem> bestStick = new List<CutItem>();
            StickRecursion(capacity, 0, cutItemsInput, ref bestStick);
            foreach (var cut in bestStick) {
                cut.StickNumber = sticknumber;
                cutItemsInput.Remove(cut);
                cutItemsOutput.Add(cut);
            }
        }

        private static void StickRecursion(float capacity, int index, List<CutItem> cutItems, ref List<CutItem> currentStick) {
            if (index >= cutItems.Count) return;
            List<CutItem> potentialStick = currentStick.ToList();
            if (index > 0) {
                if (potentialStick.Sum() + cutItems[index].Length > capacity) {
                    potentialStick.RemoveAt(potentialStick.Count - 1);
                }
            }
            for (int i = 0; i < cutItems[index].Qty; i++) {
                potentialStick.Add(cutItems[index]);
                if (potentialStick.Sum() > capacity) {
                    potentialStick.RemoveAt(potentialStick.Count - 1);
                    break;
                } else if (potentialStick.Sum() <= capacity && i == cutItems[index].Qty - 1) {
                    currentStick = potentialStick;
                    break;
                }
            }
            if (currentStick.Sum() == capacity) return;
            for (int j = index + 1; j < cutItems.Count; j++) {
                StickRecursion(capacity, j, cutItems, ref potentialStick);
            }
            if (potentialStick.Sum() > currentStick.Sum()) {
                currentStick = potentialStick;
            }
        }

        public void SortCuts(List<CutItem> cutList) {
            List<CutItem> temp = cutList.ToList();
            //foreach (CutItem item in cutList) {
            //    for (int i = 0; i < item.Qty; i++) {
            //        CutItem c = item.Clone();
            //        c.Qty = 1;
            //        c.StickNumber = 0;
            //        temp.Add(c);
            //    }
            //}
            temp.Sort();
            List<int> stockItems = new List<int>();
            foreach (var cut in temp) {
                if (!stockItems.Contains(cut.StockItemID)) {
                    stockItems.Add(cut.StockItemID);
                }
            }
            List<List<CutItem>> stratifiedCutList = new List<List<CutItem>>();
            foreach (var stockItemID in stockItems) {
                stratifiedCutList.Add(temp.FindAll(c => c.StockItemID == stockItemID));
            }
            
            List<List<CutItem>> stratifiedCutListOutput = new List<List<CutItem>>();
            foreach (var list in stratifiedCutList) {
                List<CutItem> cutItems = new List<CutItem>();
                int stickNo = 1;
                while (list.Count > 0) {
                    BestStick(stickNo, list[0].StockItem.StockLengthInInches, list, cutItems);
                    stickNo++;
                }
                stratifiedCutListOutput.Add(cutItems);
            }

            temp.Clear();
            foreach (var list in stratifiedCutListOutput) {
                foreach (var item in list) {
                    temp.Add(item);
                }
            }

            Consolidate(temp);

            cutList.Clear();
            cutList = temp.ToList();
            //foreach (var item in cutCounts) {
            //    Debug.Print(item.Key.Item1 + ", Length: " + item.Key.Item2 + ", Qty: " + item.Value + "\n");
            //}
        }

        public void Consolidate(List<CutItem> cList) {
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

        public void AddCutItem(List<CutItem> tempCutList, List<SheetCutItem> tempSheetCutList, Feature thisFeat) {
            CustomPropertyManager CustomPropMgr = default(CustomPropertyManager);
            CustomPropMgr = thisFeat.CustomPropertyManager;
            List<string> vCustomPropNames = new((string[])CustomPropMgr.GetNames());
            for (int i = 0; i < vCustomPropNames.Count; i++) {
                vCustomPropNames[i] = vCustomPropNames[i].ToLower();
            }
            if (!vCustomPropNames.Contains("description")) {
                vCustomPropNames.Add("description");
            }
            if ((vCustomPropNames.Count != 1)) {
                int i = 0;
                int qty = 0;
                float length = 0;
                float width = 0;
                float thickness = 0;
                float angle1 = 0;
                float angle2 = 0;
                string angleDirection = "-";
                string angleRotation = "-";
                string description = "";
                string material = "";
                string finish = "";
                bool isNew = false;
                Grain grainDirection = Grain.none;
                StockItem sItem = null;
                SheetStockItem sSItem = null;


                for (i = 0; i <= (vCustomPropNames.Count - 1); i++) {
                    string CustomPropName = vCustomPropNames[i];

                    string CustomPropResolvedVal;
                    CustomPropMgr.Get2(CustomPropName, out _, out CustomPropResolvedVal);
                    //Debug.Print("\t\t" + CustomPropName + ": " + CustomPropResolvedVal);
                    switch (CustomPropName.ToLower()) {
                        case "quantity":
                            Int32.TryParse(CustomPropResolvedVal, out qty);
                            break;
                        case "description":
                            description = CustomPropResolvedVal;
                            if (description == "") description = "no description property in file!";
                            break;
                        case "length":
                            float.TryParse(CustomPropResolvedVal, out length);
                            break;
                        case "bounding box length":
                            float.TryParse(CustomPropResolvedVal, out length);
                            break;
                        case "bounding box width":
                            float.TryParse(CustomPropResolvedVal, out width);
                            break;
                        case "sheet metal thickness":
                            float.TryParse(CustomPropResolvedVal, out thickness);
                            break;
                        case "surface treatment":
                            finish = CustomPropResolvedVal;
                            break;
                        case "grain":
                            if (CustomPropResolvedVal == "length") {
                                grainDirection = Grain.Length;
                            } else if (CustomPropResolvedVal == "width") {
                                grainDirection = Grain.Width;
                            }
                            finish = CustomPropResolvedVal;
                            break;
                        case "angle1":
                            if (CustomPropResolvedVal.Length > 0) {
                                float.TryParse(CustomPropResolvedVal.Substring(0, CustomPropResolvedVal.Length - 1), out angle1);
                                break;
                            }
                            angle1 = 0;
                            break;
                        case "angle2":
                            if (CustomPropResolvedVal.Length > 0) {
                                float.TryParse(CustomPropResolvedVal.Substring(0, CustomPropResolvedVal.Length - 1), out angle2);
                                break;
                            }
                            angle2 = 0;
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
                Vendor vendor;
                if (thickness != 0) {
                    var sSItems = ctx.SheetStockItems.Include(i => i.Vendor).ToList();
                    sSItem = sSItems.FirstOrDefault(item => item.InternalDescription == description);
                    if (sSItem == null) {
                        isNew = true;
                    }
                    if (isNew) {
                        if (ctx.Vendors.Any()) {
                            vendor = ctx.Vendors.AsEnumerable().ElementAt(0);
                        } else {
                            vendor = new Vendor("N/A", "N/A", "N/A", "N/A");
                            vendor.ID = 1;
                            ctx.Vendors.Add(vendor);
                            Vendors.Add(vendor);
                            ctx.SaveChanges();
                        }
                        MaterialType mat;
                        if (material.ToLower() == "material <not specified>") {
                            mat = StockItem.MaterialFromDescription(description);
                        } else {
                            mat = StockItem.MaterialFromDescription(material);
                        }
                        sSItem = new SheetStockItem(vendor: vendor, internalDescription: description,
                                             externalDescription: description,
                                             materialType: mat, thickness: thickness, finish: finish);
                        ctx.SheetStockItems.Add(sSItem);
                        SheetStockItems.Add(sSItem);
                        ctx.Vendors.Update(vendor);
                        ctx.SaveChanges();
                        isNew = false;
                    }

                    SheetCutItem sCItem = new SheetCutItem(sSItem, qty, length, width, grainDirection);
                    tempSheetCutList.Add(sCItem);

                } else {
                    var sItems = ctx.StockItems.Include(i => i.Vendor).ToList();
                    sItem = sItems.FirstOrDefault(item => item.InternalDescription == description);
                    if (sItem == null) {
                        isNew = true;
                    }
                    if (isNew) {
                        if (ctx.Vendors.Any()) {
                            vendor = ctx.Vendors.AsEnumerable().ElementAt(0);
                        } else {
                            vendor = new Vendor("N/A", "N/A", "N/A", "N/A");
                            vendor.ID = 1;
                            ctx.Vendors.Add(vendor);
                            Vendors.Add(vendor);
                            ctx.SaveChanges();
                        }
                        MaterialType mat;
                        if (material == "material <not specified>") {
                            mat = StockItem.MaterialFromDescription(description);
                        } else {
                            mat = StockItem.MaterialFromDescription(material);
                        }
                        sItem = new StockItem(vendor: vendor, internalDescription: description,
                       externalDescription: description,
                       materialType: mat,
                       profType: StockItem.ProfileFromDescription(description));
                        ctx.StockItems.Add(sItem);
                        StockItems.Add(sItem);
                        ctx.Vendors.Update(vendor);
                        ctx.SaveChanges();
                        isNew = false;
                    }

                    CutItem cItem = new CutItem(sItem, qty, length, angle1, angle2, angleDirection, angleRotation);
                    tempCutList.Add(cItem);
                }
            }
        }

        public void FindCutlist(List<CutItem> cutList, List<SheetCutItem> sheetCutList, Feature thisFeat, string parentName, string config) {
            if (hadError) return;
            int BodyCount = 0;

            string FeatType = null;
            string s = thisFeat.Name;
            FeatType = thisFeat.GetTypeName();
            if ((FeatType == "SolidBodyFolder") & (parentName == "Root Feature")) {
                inBodyFolder = true;
            }
            if ((FeatType != "SolidBodyFolder") & (parentName == "Root Feature")) {
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
                bool[] isSup = (bool[])thisFeat.IsSuppressed2((int)swInConfigurationOpts_e.swThisConfiguration, null);
                if (isSup[0]) {
                    return;
                }
                BodyFolder BodyFolder = default(BodyFolder);
                BodyFolder = (BodyFolder)thisFeat.GetSpecificFeature2();
                BodyCount = BodyFolder.GetBodyCount();
                if ((FeatType == "CutListFolder") & (BodyCount < 1)) {
                    //When BodyCount = 0, this cut list folder is not displayed in the
                    //FeatureManager design tree, so skip it
                    //return;
                }
            }

            if (FeatType == "CutListFolder") {
                //Debug.Print("\t" + s);
                if (BodyCount > 0) {
                    object obj = thisFeat.IsSuppressed2((int)swInConfigurationOpts_e.swThisConfiguration, null);
                    if (obj != null) AddCutItem(cutList, sheetCutList, thisFeat);
                }
            }
        }

        public void TraverseComponent(List<CutItem> tempCList, List<SheetCutItem> tempSCList, Component2 swComp, Action<List<CutItem>, List<SheetCutItem>, Feature, bool, string, string> action) {
            if (hadError) return;
            object[] vChildComp;
            Component2 swChildComp;

            vChildComp = (object[])swComp.GetChildren();
            if (vChildComp.Length > 0) {
                for (int i = 0; i < vChildComp.Length; i++) {
                    swChildComp = (Component2)vChildComp[i];
                    if (swChildComp.GetSuppression() == 2) TraverseComponent(tempCList, tempSCList, swChildComp, action);
                }
            } else {

                ModelDoc2 model = swComp.GetModelDoc2() as ModelDoc2;
                string config = swComp.ReferencedConfiguration;
                model.ShowConfiguration2(config);
                //Debug.Print(swComp.Name + "<" + config + ">");
                bool b = model.ForceRebuild3(false);
                //Debug.Print(b.ToString());
                Feature feature = (Feature)model.FirstFeature();
                action(tempCList, tempSCList, feature, true, "Root Feature", swComp.ReferencedConfiguration);
            }
        }

        public void TraverseFeatures(List<CutItem> tempCList, List<SheetCutItem> tempSCList, Feature thisFeat, bool isTopLevel, string parentName, string config) {
            if (hadError) return;
            Feature curFeat = default(Feature);
            curFeat = thisFeat;
            while (curFeat != null) {
                if (tempCList != null || tempSCList != null) {
                    FindCutlist(tempCList, tempSCList, curFeat, parentName, config);
                }
                Feature subfeat = default(Feature);
                subfeat = (Feature)curFeat.GetFirstSubFeature();
                while (subfeat != null) {
                    TraverseFeatures(tempCList, tempSCList, subfeat, false, curFeat.Name, config);
                    Feature nextSubFeat = default(Feature);
                    nextSubFeat = (Feature)subfeat.GetNextSubFeature();
                    subfeat = nextSubFeat;
                    nextSubFeat = null;
                }
                Feature nextFeat = default(Feature);
                if (isTopLevel) {
                    nextFeat = (Feature)curFeat.GetNextFeature();
                } else {
                    nextFeat = null;
                }
                curFeat = (Feature)nextFeat;
            }
        }
        #endregion

        #region old SortCuts()
        //public void OldSortCuts(List<CutItem> cutList) {
        //    List<CutItem> temp = new List<CutItem>();
        //    StockItem sType = null;
        //    float leftOnStick = 0;
        //    int stickNum = 1;
        //    cutList.Sort();
        //    foreach (CutItem item in cutList) {
        //        for (int i = 0; i < item.Qty; i++) {
        //            CutItem c = item.Clone();
        //            c.Qty = 1;
        //            c.StickNumber = 0;
        //            temp.Add(c);
        //        }
        //    }
        //    cutList.Clear();
        //    foreach (CutItem item in temp) {
        //        cutList.Add(item);
        //    }
        //    temp.Clear();
        //    int count = cutList.Count;

        //    for (int i = count - 1; i > -1; i--) {
        //        //First item initialization
        //        if (i == count - 1) {
        //            sType = cutList[i].StockItem;
        //            cutList[i].StickNumber = stickNum;
        //            leftOnStick = cutList[i].StockItem.StockLengthInInches - cutList[i].Length;
        //            bool isLarger = false;
        //            while (leftOnStick <= 0) {
        //                stickNum++;
        //                leftOnStick += cutList[i].StockItem.StockLengthInInches;
        //                isLarger = true;
        //            }
        //            if (isLarger) stickNum++;
        //            temp.Add(cutList[i]);
        //            cutList.RemoveAt(i);
        //            //If still same stock type as previous
        //        } else if (cutList[i].StockItem == sType) {
        //            //If there is enough left on the current stick
        //            if (leftOnStick >= cutList[i].Length) {
        //                leftOnStick -= cutList[i].Length;
        //                cutList[i].StickNumber = stickNum;
        //                if (leftOnStick == 0) {
        //                    stickNum++;
        //                    leftOnStick = cutList[i].StockItem.StockLengthInInches;
        //                }
        //                temp.Add(cutList[i]);
        //                cutList.RemoveAt(i);
        //                //If the piece is longer than the stock length
        //            } else if (cutList[i].Length > sType.StockLengthInInches) {
        //                sType = cutList[i].StockItem;
        //                leftOnStick = cutList[i].StockItem.StockLengthInInches - cutList[i].Length;
        //                cutList[i].StickNumber = stickNum;
        //                bool isLarger = false;
        //                while (leftOnStick <= 0) {
        //                    stickNum++;
        //                    leftOnStick += cutList[i].StockItem.StockLengthInInches;
        //                    isLarger = true;
        //                }
        //                if (isLarger) stickNum++;
        //                temp.Add(cutList[i]);
        //                cutList.RemoveAt(i);
        //                //If the piece is longer than current stick
        //            } else {
        //                bool isBroken = false;
        //                bool areMoreOfStockItem = false;
        //                //check what other cuts may fit on the current stick and remove them
        //                for (int j = i - 1; j > -1; j--) {
        //                    if (cutList[j].StockItem == sType) {
        //                        areMoreOfStockItem = true;
        //                        if (leftOnStick >= cutList[j].Length) {
        //                            leftOnStick -= cutList[j].Length;
        //                            cutList[j].StickNumber = stickNum;
        //                            if (leftOnStick == 0) {
        //                                stickNum++;
        //                                leftOnStick = cutList[i].StockItem.StockLengthInInches;
        //                            }
        //                            temp.Add(cutList[j]);
        //                            cutList.RemoveAt(j);
        //                            i--;
        //                            isBroken = true;
        //                        }
        //                    }
        //                }
        //                //if no other cuts will fit on current stick, go to full stock length and retry current iteration
        //                if (!isBroken && areMoreOfStockItem) {
        //                    leftOnStick = cutList[i].StockItem.StockLengthInInches;
        //                    stickNum++;
        //                    i++;
        //                    continue;
        //                    //if other cuts fit on current stick, retry current iteration
        //                } else if (isBroken) {
        //                    continue;
        //                    //if no more cuts fit on the stick
        //                } else {
        //                    stickNum++;
        //                    cutList[i].StickNumber = stickNum;
        //                    temp.Add(cutList[i]);
        //                    cutList.RemoveAt(i);
        //                    continue;
        //                }
        //            }
        //            // If new StockItem
        //        } else {
        //            sType = cutList[i].StockItem;
        //            stickNum = 1;
        //            leftOnStick = cutList[i].StockItem.StockLengthInInches - cutList[i].Length;
        //            cutList[i].StickNumber = stickNum;
        //            bool isLarger = false;
        //            while (leftOnStick <= 0) {
        //                stickNum++;
        //                leftOnStick += cutList[i].StockItem.StockLengthInInches;
        //                isLarger = true;
        //            }
        //            if (isLarger) stickNum++;
        //            temp.Add(cutList[i]);
        //            cutList.RemoveAt(i);
        //        }
        //    }


        //    Consolidate(temp);
        //    temp.Sort();
        //    cutList.Clear();
        //    foreach (CutItem item in temp) {
        //        cutList.Add(item);
        //    }
        //    //foreach (var item in cutCounts) {
        //    //    Debug.Print(item.Key.Item1 + ", Length: " + item.Key.Item2 + ", Qty: " + item.Value + "\n");
        //    //}
        //}
        #endregion

    }
}
