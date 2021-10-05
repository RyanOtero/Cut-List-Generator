using SolidPrice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidPrice.Utils {
    public static class ExtensionMethods {

        public static float Sum(this IEnumerable<CutItem> cutItems) {
            float num = 0;
            foreach (CutItem item in cutItems) {
                num += item.Length * item.Qty;
            }
            return num;
        }

    }
}
