using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Model {


    public enum MaterialType { steel, aluminum, stainless_steel, titanium, brass, bronze, zinc, copper, nickel, wood }
    public enum ProfileType { solid_square, solid_rectangular, solid_round, solid_triangular, square_tube, rectangular_tube, round_tube, pipe, custom }
    public class StockItem {
        public int ID { get; set; }

        private static List<StockItem> uniqueInstances;
        public static List<StockItem> UniqueInstances {
            get {
                if (uniqueInstances == null) {
                    uniqueInstances = new List<StockItem>();
                }
                return uniqueInstances;
            }
            set { uniqueInstances = value; }
        }

        public MaterialType MatType { get; set; }
        public ProfileType ProfType { get; set; }
        public string Series { get; set; }
        public decimal CostPerFoot { get; set; }
        public int StockLength { get; set; }
        public string Description { get; set; }
        public Vendor Vendor { get; set; }
        public decimal CostPerLength { get { return CostPerFoot * StockLength; } }


        public static StockItem CreateStockItem(Vendor vendor = null, MaterialType mType = MaterialType.steel, ProfileType profType = ProfileType.square_tube, string series = "", decimal costPerFoot = 0m, int stockLength = 24, string description = "") {
            if (UniqueInstances.Exists(x => x.Description == description)) {
                return UniqueInstances.Find(x => x.Description == description);
            }
            else {
                StockItem item = new StockItem(vendor, mType, profType, series, costPerFoot, stockLength, description);
                UniqueInstances.Add(item);
                return item;
            }
        }
        private StockItem() {

        }

        private StockItem(Vendor vendor, MaterialType mType, ProfileType profType, string series, decimal costPerFoot, int stockLength, string description) {
            MatType = mType;
            ProfType = profType;
            Series = series;
            CostPerFoot = costPerFoot;
            StockLength = stockLength;
            Description = description;
            if (vendor == null) {
                Vendor = new Vendor();
            }
            else {
                Vendor = vendor;
            }
        }
    }
}
