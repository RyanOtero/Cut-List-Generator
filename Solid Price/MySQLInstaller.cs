using System.ComponentModel;
using System.Configuration.Install;

namespace SolidPrice {
    [RunInstaller(true)]
    public partial class MySQLInstaller : Installer {
        public MySQLInstaller() {
            //    InitializeComponent();
        }
    }
}
