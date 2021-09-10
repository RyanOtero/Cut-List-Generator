using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SolidPrice.Models {
    public class SheetStockItem : IEquatable<SheetStockItem>, INotifyPropertyChanged {

        #region Fields
        private MaterialType matType;
        private float stockLengthInInches;
        private float stockWidthInInches;
        private float thickness;
        private string finish;
        private string internalDescription;
        private string externalDescription;
        private Vendor vendor;
        private decimal costPerSqFoot;
        private string vendorItemNumber;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public int ID { get; set; }

        public MaterialType MatType {
            get => matType;
            set {
                matType = value;
                OnPropertyChanged();
            }
        }

        public float StockLengthInInches {
            get => stockLengthInInches;
            set {
                stockLengthInInches = value;
                OnPropertyChanged();
            }
        }
        public float StockWidthInInches {
            get => stockWidthInInches;
            set {
                stockWidthInInches = value;
                OnPropertyChanged();
            }
        }

        public float Thickness {
            get => thickness;
            set {
                thickness = value;
                OnPropertyChanged();
            }
        }

        public string Finish {
            get => finish;
            set {
                finish = value;
                OnPropertyChanged();
            }
        }

        public string InternalDescription {
            get => internalDescription;
            set {
                internalDescription = value;
                OnPropertyChanged();
            }
        }
        public string ExternalDescription {
            get => externalDescription;
            set {
                externalDescription = value;
                OnPropertyChanged();
            }
        }
        [ForeignKey("VendorID")]
        public virtual Vendor Vendor {
            get => vendor;
            set {
                vendor = value;
                OnPropertyChanged();
            }
        }
        public decimal CostPerSqFoot {
            get => costPerSqFoot;
            set {
                costPerSqFoot = value;
                OnPropertyChanged();
            }
        }

        public string VendorItemNumber {
            get => vendorItemNumber;
            set {
                vendorItemNumber = value;
                OnPropertyChanged();
            }
        }

        public string VendorName { get { return Vendor?.VendorName; } }
        public decimal CostPerSheet => CostPerSqFoot * ((decimal)StockLengthInInches * (decimal)StockWidthInInches / 144);
        public string MatTypeString => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(MatType.ToString().Replace("_", " "));
        public string CostPerSqFootString => string.Format("{0:c}", CostPerSqFoot);
        public string CostPerSheetString => string.Format("{0:c}", CostPerSheet);
        #endregion

        #region Constructors
        public SheetStockItem() { }

        public SheetStockItem(Vendor vendor = null, MaterialType materialType = MaterialType.steel,
            decimal costPerSqFoot = 0m, float stockLength = 96, float stockWidth = 48, float thickness = 0, string finish = "", string internalDescription = "", string externalDescription = "", string vendorItemNumber = "") {
            MatType = materialType;
            CostPerSqFoot = costPerSqFoot;
            StockLengthInInches = stockLength;
            StockWidthInInches = stockWidth;
            Thickness = thickness;
            Finish = finish;
            InternalDescription = internalDescription;
            ExternalDescription = externalDescription;
            Vendor = vendor;
            VendorItemNumber = vendorItemNumber;
        }
        #endregion

        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region From Description Methods
        public static MaterialType MaterialFromDescription(string desc) {
            MaterialType mType = MaterialType.none;
            bool breaker = false;
            string[] descArray = desc.Split(" ");
            MaterialType[] types = (MaterialType[])Enum.GetValues(typeof(MaterialType));

            foreach (MaterialType t in types) {
                if (breaker) break;

                switch (t) {
                    case MaterialType.stainless_steel:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("stainless") || str.ToLower() == "ss") {
                                mType = MaterialType.stainless_steel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.steel:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "steel") {
                                mType = MaterialType.steel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.titanium:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "titanium") {
                                mType = MaterialType.titanium;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.brass:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "brass") {
                                mType = MaterialType.brass;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.bronze:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "bronze") {
                                mType = MaterialType.bronze;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.zinc:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "zinc") {
                                mType = MaterialType.zinc;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.copper:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "copper") {
                                mType = MaterialType.copper;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.nickel:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "nickel") {
                                mType = MaterialType.nickel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.wood:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "wood" || str.ToLower() == "lumber") {
                                mType = MaterialType.wood;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.aluminum:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("alum")) {
                                mType = MaterialType.aluminum;
                                breaker = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return mType;
        }

        #endregion

        #region Comparison Methods
        public int CompareTo(SheetStockItem other) {
            if (other != null) {
                if (ID == other.ID) {
                    if (Vendor.ID == other.Vendor.ID) {
                        if (InternalDescription == other.InternalDescription) {
                            if (ExternalDescription == other.ExternalDescription) {
                                if (CostPerSqFoot == other.CostPerSqFoot) {
                                    if (MatType == other.MatType) {
                                        if (Finish == other.Finish) {
                                            return StockLengthInInches.CompareTo(other.StockLengthInInches);
                                        } else {
                                            Finish.CompareTo(other.Finish);
                                        }
                                    } else {
                                        MatType.CompareTo(other.MatType);
                                    }
                                } else {
                                    return CostPerSqFoot.CompareTo(other.CostPerSqFoot);
                                }
                            } else {
                                return ExternalDescription.CompareTo(other.ExternalDescription);
                            }
                        } else {
                            return InternalDescription.CompareTo(other.InternalDescription);
                        }
                    } else {
                        return Vendor.ID.CompareTo(other.Vendor.ID);
                    }
                } else {
                    return ID.CompareTo(other.ID);
                }
            }
            return 1;
        }

        public override bool Equals(object obj) => this.Equals(obj as SheetStockItem);

        public override int GetHashCode() => ID.GetHashCode();

        public bool Equals(SheetStockItem other) {
            if (other is null) {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other)) {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType()) {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (ID == other.ID) &&
                (MatType == other.MatType) &&
                (StockLengthInInches == other.StockLengthInInches) &&
                (StockWidthInInches == other.StockWidthInInches) &&
                (Thickness == other.Thickness) &&
                (Finish == other.Finish) &&
                (InternalDescription == other.InternalDescription) &&
                (ExternalDescription == other.ExternalDescription) &&
                (Vendor == other.Vendor) &&
                (CostPerSqFoot == other.CostPerSqFoot);

        }

        public static bool operator ==(SheetStockItem left, SheetStockItem right) {
            if (left is null) {
                if (right is null) {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return left.Equals(right);
        }

        public static bool operator !=(SheetStockItem left, SheetStockItem right) => !(left == right);
    }
    #endregion
}