using System.ComponentModel;

namespace SolidPrice.Models {
    public partial class SheetCutItem {
        public enum Grain
        {
            [Description("-")]
            none,
            [Description("Length")]
            Length,
            [Description("Width")]
            Width
        }
    }
}