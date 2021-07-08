using Solidworks_Cutlist_Generator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.BusinessLogic {
    class CutSorter {
        List<CutItem> RawList { get; }

        public CutSorter(List<CutItem> rawList) {
            RawList = rawList;
        }
        public List<CutItem> SortCuts() {
            return new List<CutItem>();
        }


    }
}
