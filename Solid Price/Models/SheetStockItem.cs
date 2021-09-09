using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SolidPrice.Models
{
    public class SheetStockItem : IEquatable<SheetStockItem>, INotifyPropertyChanged
    {

        #region Fields
        private MaterialType matType;
        private float stockLength;
        private float stockWidth;
        private float thickness;
        private string internalDescription;
        private string externalDescription;
        private Vendor vendor;
        private decimal costPerSqFoot;
        private string vendorItemNumber;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public int ID { get; set; }

        public MaterialType MatType
        {
            get => matType;
            set
            {
                matType = value;
                OnPropertyChanged();
            }
        }

        public float StockLength
        {
            get => stockLength;
            set
            {
                stockLength = value;
                OnPropertyChanged();
            }
        }
        public float StockWidth
        {
            get => stockWidth;
            set
            {
                stockWidth = value;
                OnPropertyChanged();
            }
        }

        public float Thickness
        {
            get => thickness;
            set
            {
                thickness = value;
                OnPropertyChanged();
            }
        }

        public string InternalDescription
        {
            get => internalDescription;
            set
            {
                internalDescription = value;
                OnPropertyChanged();
            }
        }
        public string ExternalDescription
        {
            get => externalDescription;
            set
            {
                externalDescription = value;
                OnPropertyChanged();
            }
        }
        [ForeignKey("VendorID")]
        public virtual Vendor Vendor
        {
            get => vendor;
            set
            {
                vendor = value;
                OnPropertyChanged();
            }
        }
        public decimal CostPerSqFoot
        {
            get => costPerSqFoot;
            set
            {
                costPerSqFoot = value;
                OnPropertyChanged();
            }
        }

        public string VendorItemNumber
        {
            get => vendorItemNumber;
            set
            {
                vendorItemNumber = value;
                OnPropertyChanged();
            }
        }

        public string VendorName { get { return Vendor?.VendorName; } }
        public decimal CostPerSheet => CostPerSqFoot * (decimal)StockLength * (decimal)StockWidth;
        public float StockLengthInInches => StockLength * 12;
        public string MatTypeString => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(MatType.ToString().Replace("_", " "));
        public string CostPerSqFootString => string.Format("{0:c}", CostPerSqFoot);
        public string CostPerSheetString => string.Format("{0:c}", CostPerSheet);
        #endregion

        #region Constructors
        public SheetStockItem() { }

        public SheetStockItem(Vendor vendor = null, MaterialType materialType = MaterialType.steel,
            ProfileType profType = ProfileType.square_tube, decimal costPerFoot = 0m,
            float stockLength = 24, string internalDescription = "", string externalDescription = "", string vendorItemNumber = "")
        {
            MatType = materialType;
            CostPerSqFoot = costPerSqFoot;
            StockLength = stockLength;
            StockWidth = stockWidth;
            InternalDescription = internalDescription;
            ExternalDescription = externalDescription;
            Vendor = vendor;
            VendorItemNumber = vendorItemNumber;
        }
        #endregion

        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region From Description Methods
        public static MaterialType MaterialFromDescription(string desc)
        {
            MaterialType mType = MaterialType.none;
            bool breaker = false;
            string[] descArray = desc.Split(" ");
            MaterialType[] types = (MaterialType[])Enum.GetValues(typeof(MaterialType));

            foreach (MaterialType t in types)
            {
                if (breaker) break;

                switch (t)
                {
                    case MaterialType.stainless_steel:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower().Contains("stainless") || str.ToLower() == "ss")
                            {
                                mType = MaterialType.stainless_steel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.steel:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "steel")
                            {
                                mType = MaterialType.steel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.titanium:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "titanium")
                            {
                                mType = MaterialType.titanium;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.brass:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "brass")
                            {
                                mType = MaterialType.brass;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.bronze:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "bronze")
                            {
                                mType = MaterialType.bronze;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.zinc:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "zinc")
                            {
                                mType = MaterialType.zinc;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.copper:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "copper")
                            {
                                mType = MaterialType.copper;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.nickel:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "nickel")
                            {
                                mType = MaterialType.nickel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.wood:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower() == "wood" || str.ToLower() == "lumber")
                            {
                                mType = MaterialType.wood;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.aluminum:
                        foreach (string str in descArray)
                        {
                            if (str.ToLower().Contains("alum"))
                            {
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
        public int CompareTo(SheetStockItem other)
        {
            if (other != null)
            {
                if (ID == other.ID)
                {
                    if (Vendor.ID == other.Vendor.ID)
                    {
                        if (InternalDescription == other.InternalDescription)
                        {
                            if (ExternalDescription == other.ExternalDescription)
                            {
                                if (CostPerFoot == other.CostPerFoot)
                                {
                                    if (MatType == other.MatType)
                                    {
                                        if (ProfType == other.ProfType)
                                        {
                                            return StockLength.CompareTo(other.StockLength);
                                        }
                                        else
                                        {
                                            ProfType.CompareTo(other.ProfType);
                                        }
                                    }
                                    else
                                    {
                                        MatType.CompareTo(other.MatType);
                                    }
                                }
                                else
                                {
                                    return CostPerFoot.CompareTo(other.CostPerFoot);
                                }
                            }
                            else
                            {
                                return ExternalDescription.CompareTo(other.ExternalDescription);
                            }
                        }
                        else
                        {
                            return InternalDescription.CompareTo(other.InternalDescription);
                        }
                    }
                    else
                    {
                        return Vendor.ID.CompareTo(other.Vendor.ID);
                    }
                }
                else
                {
                    return ID.CompareTo(other.ID);
                }
            }
            return 1;
        }

        public override bool Equals(object obj) => this.Equals(obj as SheetStockItem);

        public override int GetHashCode() => ID.GetHashCode();

        public bool Equals(SheetStockItem other)
        {
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (ID == other.ID) &&
                (MatType == other.MatType) &&
                (ProfType == other.ProfType) &&
                (StockLength == other.StockLength) &&
                (InternalDescription == other.InternalDescription) &&
                (ExternalDescription == other.ExternalDescription) &&
                (Vendor == other.Vendor) &&
                (CostPerFoot == other.CostPerFoot);

        }

        public static bool operator ==(SheetStockItem left, SheetStockItem right)
        {
            if (left is null)
            {
                if (right is null)
                {
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