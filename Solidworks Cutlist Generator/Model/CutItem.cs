using Solidworks_Cutlist_Generator.Model;
using System;

namespace Solidworks_Cutlist_Generator.BusinessLogic {
    public class CutItem : IComparable<CutItem>, IEquatable<CutItem> {

        public StockItem StockType { get; }
        public int Qty { get; set; }
        public string Description {
            get { return StockType.InternalDescription; }
        }
        public float Length { get; set; }
        public float Angle1 { get; set; }
        public float Angle2 { get; set; }
        public string AngleDirection { get; set; }
        public string AngleRotation { get; set; }
        public string Cost {
            get {
                return string.Format("{0:c}", StockType.CostPerFoot / 12m * (decimal)Length);
            }
        }

        public int StickNumber { get; set; }


        public CutItem(StockItem stockType, int qty, float length, float angle1, float angle2, string angleDirection, string angleRotation, int stickNumber = 0) {
            StockType = stockType;
            Qty = qty;
            Length = length;
            Angle1 = angle1;
            Angle2 = angle2;
            AngleDirection = angleDirection;
            AngleRotation = angleRotation;
            StickNumber = stickNumber;
        }

        public int CompareTo(CutItem item2) {
            int i = Description.CompareTo(item2.Description);
            if (i == -1) {
                return -1;
            } else if (i == 1) {
                return 1;
            } else {
                int j = StickNumber.CompareTo(item2.StickNumber);
                if (j == 1) {
                    return 1;
                } else if (j == -1) {
                    return -1;
                } else {
                    int k = Length.CompareTo(item2.Length);
                    if (k == -1) {
                        return 1;
                    } else if (k == 1) {
                        return -1;
                    } else {
                        return 0;
                    }
                }
            }

        }

        public bool Equals(CutItem other) {
            if (other.Length == Length &&
                other.Description == Description &&
                other.StockType == StockType &&
                other.Qty == Qty &&
                other.Angle1 == Angle1 &&
                other.Angle2 == Angle2 &&
                other.AngleDirection == AngleDirection &&
                other.AngleRotation == AngleRotation) {
                return true;
            }
            return false;
        }

        public CutItem Clone() {
            CutItem cItem = new CutItem(StockType, 1, Length, Angle1, Angle2, AngleDirection, AngleRotation);
            return cItem;
        }
    }
}