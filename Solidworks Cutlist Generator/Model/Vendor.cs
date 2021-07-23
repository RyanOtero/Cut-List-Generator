using static Solidworks_Cutlist_Generator.BusinessLogic.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Model {
    public class Vendor {

        public int ID { get; set; }
        public string VendorName { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }

        private Vendor() { }

        public Vendor(string vendorName = "", string phoneNumber = "", string contactName = "", string contactEmail = "") {
            VendorName = vendorName;
            PhoneNumber = phoneNumber;
            ContactName = contactName;
            ContactEmail = contactEmail;
        }
    }
}
