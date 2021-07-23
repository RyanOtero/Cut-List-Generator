using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Model {


    public enum MaterialType { none, steel, aluminum, stainless_steel, titanium, brass, bronze, zinc, copper, nickel, wood }
    public enum ProfileType { custom, angle, channel, flat_bar, solid_square, solid_rectangular, solid_round, solid_triangular, square_tube, rectangular_tube, round_tube, pipe }
    public class StockItem : IEquatable<StockItem> {
        public int ID { get; set; }
        public MaterialType MatType { get; set; }
        public ProfileType ProfType { get; set; }
        public float StockLength { get; set; }
        public string InternalDescription { get; set; }
        public string ExternalDescription { get; set; }
        public Vendor Vendor { get; set; }
        public decimal CostPerFoot { get; set; }
        public decimal CostPerLength { get { return CostPerFoot * (decimal)StockLength; } }

        public float StockLengthInInches {
            get {
                return StockLength * 12;
            }
        }
        public int VendorIdentifier {
            get {
                return Vendor.ID;
            }
        }

        public string MatTypeString {
            get {
                return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(MatType.ToString().Replace("_", " "));
            }
        }

        public string ProfTypeString {
            get {
                return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(ProfType.ToString().Replace("_", " "));
            }
        }

        public string CostPerFootString {
            get {
                return string.Format("{0:c}", CostPerFoot);
            }
        }

        public string CostPerLengthString {
            get {
                return string.Format("{0:c}", CostPerLength);
            }
        }

        public StockItem() { }

        public StockItem(Vendor vendor = null, MaterialType materialType = MaterialType.steel,
            ProfileType profType = ProfileType.square_tube, string series = "", decimal costPerFoot = 0m,
            float stockLength = 24, string internalDescription = "", string externalDescription = "") {
            MatType = materialType;
            ProfType = profType;
            CostPerFoot = costPerFoot;
            StockLength = stockLength;
            InternalDescription = internalDescription;
            ExternalDescription = externalDescription;
            Vendor = vendor;
        }

        public static MaterialType MaterialFromDescription(string desc) {
            MaterialType mType = MaterialType.none;
            bool breaker = false;
            string[] descArray = desc.Split(" ");
            MaterialType[] types = (MaterialType[])Enum.GetValues(typeof(MaterialType));

            foreach (MaterialType t in types) {
                if (breaker) break;

                switch (t) {
                    case MaterialType.stainless_steel:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("stainless") || str.ToLower() == "ss") {
                                mType = MaterialType.stainless_steel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.steel:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "steel") {
                                mType = MaterialType.steel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.titanium:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "titanium") {
                                mType = MaterialType.titanium;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.brass:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "brass") {
                                mType = MaterialType.brass;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.bronze:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "bronze") {
                                mType = MaterialType.bronze;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.zinc:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "zinc") {
                                mType = MaterialType.zinc;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.copper:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "copper") {
                                mType = MaterialType.copper;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.nickel:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "nickel") {
                                mType = MaterialType.nickel;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.wood:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "wood" || str.ToLower() == "lumber") {
                                mType = MaterialType.wood;
                                breaker = true;
                            }
                        }
                        break;
                    case MaterialType.aluminum:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("alum")) {
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

        public static ProfileType ProfileFromDescription(string desc) {
            ProfileType pType = ProfileType.custom;
            bool breaker = false;
            string[] descArray = desc.Split(" ");
            ProfileType[] types = (ProfileType[])Enum.GetValues(typeof(ProfileType));

            bool isSolid = false;
            bool isTube = false;
            bool isSquare = false;
            bool isRect = false;
            bool isTri = false;
            bool isRound = false;

            foreach (ProfileType t in types) {
                if (breaker) break;
                switch (t) {
                    case ProfileType.angle:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "angle") {
                                pType = ProfileType.angle;
                                breaker = true;
                            }
                        }
                        break;
                    case ProfileType.channel:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "channel") {
                                pType = ProfileType.channel;
                                breaker = true;
                            }
                        }
                        break;
                    case ProfileType.flat_bar:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "flat" || str.ToLower() == "fb") {
                                pType = ProfileType.flat_bar;
                                breaker = true;
                            }
                        }
                        break;
                    case ProfileType.solid_square:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "square") {
                                isSquare = true;
                            }
                            if (str.ToLower() == "solid" || str.ToLower() == "sld") {
                                isSolid = true;
                            }
                        }
                        if (isSquare && isSolid) {
                            pType = ProfileType.solid_square;
                            breaker = true;
                        }
                        break;
                    case ProfileType.solid_rectangular:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("rect")) {
                                isRect = true;
                            }
                            if (str.ToLower() == "solid" || str.ToLower() == "sld") {
                                isSolid = true;
                            }
                        }
                        if (isRect && isSolid) {
                            pType = ProfileType.solid_rectangular;
                            breaker = true;
                        }
                        break;
                    case ProfileType.solid_round:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "round") {
                                isRound = true;
                            }
                            if (str.ToLower() == "solid" || str.ToLower() == "sld") {
                                isSolid = true;
                            }
                        }
                        if (isRound && isSolid) {
                            pType = ProfileType.solid_round;
                            breaker = true;
                        }
                        break;
                    case ProfileType.solid_triangular:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("triang")) {
                                isTri = true;
                            }
                            if (str.ToLower() == "solid" || str.ToLower() == "sld") {
                                isSolid = true;
                            }
                        }
                        if (isTri && isSolid) {
                            pType = ProfileType.solid_triangular;
                            breaker = true;
                        }
                        break;
                    case ProfileType.square_tube:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "square") {
                                isSquare = true;
                            }
                            if (str.ToLower() == "tube" || str.ToLower() == "tubing") {
                                isTube = true;
                            }
                        }
                        if (isSquare && isTube) {
                            pType = ProfileType.square_tube;
                            breaker = true;
                        }
                        break;
                    case ProfileType.rectangular_tube:
                        foreach (string str in descArray) {
                            if (str.ToLower().Contains("rect")) {
                                isRect = true;
                            }
                            if (str.ToLower() == "tube" || str.ToLower() == "tubing") {
                                isTube = true;
                            }
                        }
                        if (isRect && isTube) {
                            pType = ProfileType.rectangular_tube;
                            breaker = true;
                        }
                        break;
                    case ProfileType.round_tube:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "round") {
                                isRound = true;
                            }
                            if (str.ToLower() == "tube" || str.ToLower() == "tubing") {
                                isTube = true;
                            }
                        }
                        if ((isRound || (!isSquare && !isRect)) && isTube) {
                            pType = ProfileType.round_tube;
                            breaker = true;
                        }
                        break;
                    case ProfileType.pipe:
                        foreach (string str in descArray) {
                            if (str.ToLower() == "pipe") {
                                pType = ProfileType.pipe;
                            }
                        }
                        break;
                    default:
                        pType = ProfileType.custom;
                        break;
                }
            }
            return pType;
        }

        public override bool Equals(object obj) => this.Equals(obj as StockItem);

        public override int GetHashCode() => ID.GetHashCode();

        public bool Equals(StockItem other) {
            if (other is null) {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other)) {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType()) {
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

        public static bool operator ==(StockItem left, StockItem right) {
            if (left is null) {
                if (right is null) {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return left.Equals(right);
        }

        public static bool operator !=(StockItem left, StockItem right) => !(left == right);
    }
}