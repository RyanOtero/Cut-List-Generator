using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Models {
    class OrderItem : IComparable<OrderItem>, IEquatable<OrderItem>, INotifyPropertyChanged {
        private StockItem stockItem;
        private int qty;

        public int ID { get; set; }
        public int Qty {
            get => qty;
            set {
                qty = value;
                OnPropertyChanged();
            }
        }

        [ForeignKey("StockItemID")]
        public StockItem StockItem {
            get => stockItem;
            set {
                stockItem = value;
                OnPropertyChanged();
            }
        }
        public string Description { get { return StockItem.ExternalDescription; } }
        public float StockLengthInFeet { get { return StockItem.StockLength; } }
        public decimal CostPerLength { get { return StockItem.CostPerLength; } }
        public string VendorName { get { return StockItem.VendorName; } }
        public string CostPerLengthString { get { return StockItem.CostPerLengthString; } }
        public string TotalCost { get { return string.Format("{0:c}", CostPerLength * Qty); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public OrderItem() { }

        public OrderItem(int qty, StockItem stockItem) {
            Qty = qty;
            StockItem = stockItem;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(OrderItem other) {
            int i = Qty.CompareTo(other.Qty);
            if (i == 1) {
                return 1;
            } else if (i == 1) {
                return 1;
            } else {
                return StockItem.CompareTo(other.StockItem);
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
