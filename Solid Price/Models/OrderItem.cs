using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace SolidPrice.Models {
    public class OrderItem : IComparable<OrderItem>, IEquatable<OrderItem>, INotifyPropertyChanged {
        private StockItem stockItem;
        private int qty;
        private int stockItemID;

        public int ID { get; set; }

        [ForeignKey("StockItemID")]
        public int StockItemID {
            get => stockItemID;
            set {
                stockItemID = value;
                OnPropertyChanged();
            }
        }

        public StockItem StockItem {
            get => stockItem;
            set {
                if (value != null) StockItemID = value.ID;
                stockItem = value;
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
            get { return StockItem?.ExternalDescription; }
        }

        public float StockLengthInFeet {
            get {
                if (StockItem == null) return 0;
                return StockItem.StockLength;
            }
        }

        public decimal CostPerLength {
            get {
                if (StockItem == null) return 0;
                return StockItem.CostPerLength;
            }
        }

        public string VendorName {
            get { return StockItem?.VendorName; }
        }

        public string CostPerLengthString => stockItem.CostPerLengthString;
        public string TotalCost => string.Format("{0:c}", Math.Round(CostPerLength * Qty, 2, MidpointRounding.ToPositiveInfinity));

        public event PropertyChangedEventHandler PropertyChanged;

        public OrderItem() { }

        public OrderItem(int qty, StockItem stockItem) {
            Qty = qty;
            StockItem = stockItem;
            StockItemID = stockItem.ID;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(OrderItem other) {
            int i = StockItem.ExternalDescription.CompareTo(other.StockItem.ExternalDescription);
            if (i == 1) {
                return 1;
            } else if (i == -1) {
                return -1;
            } else {
                return Qty.CompareTo(other.Qty);
            }

        }

        public bool Equals(OrderItem other) {
            if (other.Description == Description &&
                other.StockLengthInFeet == StockLengthInFeet &&
                other.Qty == Qty &&
                other.CostPerLength == CostPerLength &&
                other.VendorName == VendorName) {
                return true;
            }
            return false;
        }

        public OrderItem Clone() {
            return new OrderItem(Qty, StockItem);
        }
    }
}
