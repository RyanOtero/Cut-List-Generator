using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Models {
    class OrderItem {
        public string Description { get; set; }
        public float StockLengthInFeet { get; set; }
        public int Qty { get; set; }
        public decimal CostPerLength { get; set; }
        public string VendorName { get; set; }

        public decimal TotalCost {
            get {
                return CostPerLength * (decimal)StockLengthInFeet;
            }
        }



        public OrderItem(string description, float stockLengthInFeet, int qty, decimal costPerLength, string vendorName) {
            Description = description;
            StockLengthInFeet = stockLengthInFeet;
            Qty = qty;
            CostPerLength = costPerLength;
             VendorName = vendorName;
       }
    }
}
