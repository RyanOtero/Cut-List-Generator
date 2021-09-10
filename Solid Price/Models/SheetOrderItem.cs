using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SolidPrice.Models
{
    public class SheetOrderItem : IComparable<SheetOrderItem>, IEquatable<SheetOrderItem>, INotifyPropertyChanged
    {
        private SheetStockItem sheetStockItem;
        private int qty;
        private int sheetStockItemID;

        public int ID { get; set; }

        [ForeignKey("SheetStockItemID")]
        public int SheetStockItemID
        {
            get => sheetStockItemID;
            set
            {
                sheetStockItemID = value;
                OnPropertyChanged();
            }
        }

        public SheetStockItem SheetStockItem
        {
            get => sheetStockItem;
            set
            {
                if (value != null) SheetStockItemID = value.ID;
                sheetStockItem = value;
                OnPropertyChanged();
            }
        }

        public int Qty
        {
            get => qty;
            set
            {
                qty = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return SheetStockItem?.ExternalDescription; }
        }

        public float StockLengthInFeet
        {
            get
            {
                if (SheetStockItem == null) return 0;
                return SheetStockItem.StockLengthInInches;
            }
        }

        public decimal CostPerSheet
        {
            get
            {
                if (SheetStockItem == null) return 0;
                return SheetStockItem.CostPerSheet;
            }
        }

        public string VendorName
        {
            get { return SheetStockItem?.VendorName; }
        }

        public string CostPerSqFootString => SheetStockItem.CostPerSqFootString;
        public string TotalCost => string.Format("{0:c}", CostPerSheet * Qty);

        public event PropertyChangedEventHandler PropertyChanged;

        public SheetOrderItem() { }

        public SheetOrderItem(int qty, SheetStockItem sheetStockItem)
        {
            Qty = qty;
            SheetStockItem = sheetStockItem;
            SheetStockItemID = sheetStockItem.ID;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(SheetOrderItem other)
        {
            int i = SheetStockItem.ExternalDescription.CompareTo(other.SheetStockItem.ExternalDescription);
            if (i == 1)
            {
                return 1;
            }
            else if (i == -1)
            {
                return -1;
            }
            else
            {
                return Qty.CompareTo(other.Qty);
            }

        }

        public bool Equals(SheetOrderItem other)
        {
            if (other.Description == Description &&
                other.StockLengthInFeet == StockLengthInFeet &&
                other.Qty == Qty &&
                other.CostPerSheet == CostPerSheet &&
                other.VendorName == VendorName)
            {
                return true;
            }
            return false;
        }

        public SheetOrderItem Clone()
        {
            return new SheetOrderItem(Qty, SheetStockItem);
        }
    }
}
