using System.ComponentModel;

namespace SolidPrice.Models {
    public enum Grain {
        [Description("-")]
        none,
        [Description("Length")]
        Length,
        [Description("Width")]
        Width
    }
}