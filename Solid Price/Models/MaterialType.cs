using SolidPrice.Utils;
using System.ComponentModel;

namespace SolidPrice.Models {

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MaterialType {
        [Description("N/A")]
        none,
        [Description("Steel")]
        steel,
        [Description("Aluminum")]
        aluminum,
        [Description("Stainless Steel")]
        stainless_steel,
        [Description("Titanium")]
        titanium,
        [Description("Brass")]
        brass,
        [Description("Bronze")]
        bronze,
        [Description("Zinc")]
        zinc,
        [Description("Copper")]
        copper,
        [Description("Nickel")]
        nickel,
        [Description("Wood")]
        wood
    }
}