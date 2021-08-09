using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator {
    [RunInstaller(true)]
    public partial class MySQLInstaller : Installer {
        public MySQLInstaller() {
        //    InitializeComponent();
        }
    }
}
