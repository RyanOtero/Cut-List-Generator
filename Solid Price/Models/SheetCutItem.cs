using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SolidPrice.Models {
    public partial class SheetCutItem : IComparable<SheetCutItem>, IEquatable<SheetCutItem>, INotifyPropertyChanged {

        #region Fields
        private float length;
        private float width;
        private SheetStockItem sheetStockItem;
        private int sheetStockItemID;
        private int qty;
        private int sheetNumber;
        private Grain grainDirection;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public int ID { get; set; }

        [ForeignKey("SheetStockItemID")]
        public int SheetStockItemID {
            get => sheetStockItemID;
            set {
                sheetStockItemID = value;
                OnPropertyChanged();
            }
        }


        public SheetStockItem SheetStockItem {
            get => sheetStockItem;
            set {
                if (value != null) SheetStockItemID = value.ID;
                sheetStockItem = value;
                OnPropertyChanged();
            }
        }

        public int Qty {
            get => qty;
            set {
                qty = value;
                OnPropertyChanged();
            }
        }

        public string Description {
            get { return SheetStockItem?.ExternalDescription; }
        }

        public float Length {
            get => length;
            set {
                length = value;
                OnPropertyChanged();
            }
        }

        public float Width {
            get => width;
            set {
                width = value;
                OnPropertyChanged();
            }
        }

        public Grain GrainDirection
        {
            get => grainDirection;
            set {
                grainDirection = value;
                OnPropertyChanged();
            }
        }

        public int SheetNumber {
            get => sheetNumber;
            set {
                sheetNumber = value;
                OnPropertyChanged();
            }
        }

        public string Cost {
            get {
                decimal num = SheetStockItem != null ? SheetStockItem.CostPerSqFoot : 0;
                return string.Format("{0:c}", num * ((decimal)Length * (decimal)width / 144));
            }

        }

        public string TotalCost {
            get {
                decimal num = SheetStockItem != null ? SheetStockItem.CostPerSqFoot : 0;
                return string.Format("{0:c}", num / 12 * (decimal)Length * Qty);
            }
        }

        public string SheetNumberString {
            get {
                if (SheetNumber == 0) {
                    return "-";
                }
                return SheetNumber.ToString();
            }
        }
        #endregion


        public SheetCutItem() { }

        public SheetCutItem(SheetStockItem sheetStockItem, int qty, float length, float width, Grain grainDirection, int sheetNumber = 0) {
            SheetStockItem = sheetStockItem;
            SheetStockItemID = sheetStockItem.ID;
            Qty = qty;
            Length = length;
            Width = width;
            GrainDirection = grainDirection;
            SheetNumber = sheetNumber;
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(SheetCutItem other) {
            if (other != null) {
                if (Description != other.Description) {
                    if (SheetNumber != other.SheetNumber) {
                        if (Length != other.Length) {
                            return Width.CompareTo(other.Width);
                        } else {
                            return Length.CompareTo(other.Length);
                        }
                    } else {
                        return SheetNumber.CompareTo(other.SheetNumber);
                    }
                } else {
                    return Description.CompareTo(other.Description);
                }
            }
            return 1;
        }

        public bool Equals(SheetCutItem other) {
            if (other.Length == Length &&
                other.Width == Width &&
                other.Description == Description &&
                other.SheetStockItem == SheetStockItem &&
                other.Qty == Qty) {
                return true;
            }
            return false;
        }

        public SheetCutItem Clone() {
            return new SheetCutItem(SheetStockItem, Qty, Length, Width, GrainDirection, SheetNumber);
        }
    }
}