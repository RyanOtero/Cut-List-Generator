using Solidworks_Cutlist_Generator.Model;

namespace Solidworks_Cutlist_Generator.BusinessLogic {
    public class CutItem {

        public StockItem StockType { get; }
        public int Qty { get; }
        public string Description {
            get { return StockType.Description; }
        }
        public float Length { get; }
        public float Angle1 { get; }
        public float Angle2 { get; }

        public CutItem(StockItem stockType, int qty, float length, float angle1, float angle2) {
            StockType = stockType;
            Qty = qty;
            Length = length;
            Angle1 = angle1;
            Angle2 = angle2;
        }
    }
}