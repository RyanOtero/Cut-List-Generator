using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Models {
    public class OrderItem : IComparable<OrderItem>, IEquatable<OrderItem>, INotifyPropertyChanged {
        private StockItem stockItem;
        private int qty;
        private int stockItemID;
        private string description;
        private float stockLengthInFeet;
        private decimal costPerLength;
        private string vendorName;

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
            get => description;
            set {
                description = value;
                OnPropertyChanged();
            }
        }
        
        public float StockLengthInFeet {
            get => stockLengthInFeet;
            set {
                stockLengthInFeet = value;
                OnPropertyChanged();
            }
        }
        
        public Decimal CostPerLength {
            get => costPerLength;
            set {
                costPerLength = value;
                OnPropertyChanged();
            }
        }
        
        public string VendorName {
            get => vendorName;
            set {
                vendorName = value;
                OnPropertyChanged();
            }
        }

        public string CostPerLengthString { get { return string.Format("{0:c}", CostPerLength); } }
        public string TotalCost { get { return string.Format("{0:c}", CostPerLength * Qty); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public OrderItem() { }

        public OrderItem(int qty, StockItem stockItem) {
            Description = StockItem.ExternalDescription;
            StockLengthInFeet = StockItem.StockLength;
            VendorName = StockItem.VendorName;
            CostPerLength = StockItem.CostPerLength;
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
