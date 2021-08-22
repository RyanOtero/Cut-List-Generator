using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Solid_Price {
    [RunInstaller(true)]
    public partial class MySQLInstaller : Installer {
        public MySQLInstaller() {
        //    InitializeComponent();
        }
    }
}
