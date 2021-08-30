using SolidPrice.Utils;
using System.ComponentModel;

namespace SolidPrice.Models {

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ProfileType {
        [Description("N/A")]
        none,
        [Description("Custom")]
        custom,
        [Description("Angle")]
        angle,
        [Description("Channel")]
        channel,
        [Description("Flat Bar")]
        flat_bar,
        [Description("Solid Square")]
        solid_square,
        [Description("Solid Rectangular")]
        solid_rectangular,
        [Description("Solid Round")]
        solid_round,
        [Description("Solid Triangular")]
        solid_triangular,
        [Description("Solid Hexagonal")]
        solid_hexagonal,
        [Description("Square Tube")]
        square_tube,
        [Description("Rectangular Tube")]
        rectangular_tube,
        [Description("Round Tube")]
        round_tube,
        [Description("Pipe")]
        pipe
    }
}