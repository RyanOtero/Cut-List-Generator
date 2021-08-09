using Solidworks_Cutlist_Generator.Utils;
using System.ComponentModel;

namespace Solidworks_Cutlist_Generator.Models {

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