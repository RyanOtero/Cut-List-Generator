using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Solidworks_Cutlist_Generator.Utils.Messenger;

namespace Solidworks_Cutlist_Generator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        string fileName = "App.txt";

        public App() {
            // Initialize application-scope property
            this.Properties["ServerString"] = "";
            this.Properties["DatabaseString"] = "";
            this.Properties["UserString"] = "";
            this.Properties["PasswordString"] = "";
            this.Properties["UseExternalDB"] = false;
        }

        private void App_Startup(object sender, StartupEventArgs e) {
            // Restore application-scope property from isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            try {
                using IsolatedStorageFileStream stream = new(fileName, FileMode.OpenOrCreate, storage);
                using StreamReader reader = new(stream);
                bool hasContents = false;
                // Restore each application-scope property individually
                while (!reader.EndOfStream) {
                    string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                    this.Properties[keyValue[0]] = keyValue[1];
                    Debug.Print(keyValue[0] + ", " + keyValue[1]);
                    hasContents = true;
                }
                if (!hasContents) {
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;
                    string caption = "Database Needed"; 
                    string text = "A database is needed to use this application. Would you like to use an external MySQL database?\n\n" +
                        "If so, you will need a connection string. On the Configuration tab, enter a connection string in the format of:\n" +
                        "\nserver=[Server Name];database=[Database Name];user=[User Name];password=[Password]";

                    result = MessageBox.Show(text, caption, button, icon, MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes) {
                        this.Properties["UseExternalDB"] = true;
                    } else {
                        this.Properties["UseExternalDB"] = false;
                    }
                }
            } catch (Exception ) {
                // Fail silently.
            }
        }

        private void App_Exit(object sender, ExitEventArgs e) {
            // Persist application-scope property to isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
            using (StreamWriter writer = new StreamWriter(stream)) {
                // Persist each application-scope property individually
                foreach (string key in this.Properties.Keys) {
                    writer.WriteLine("{0},{1}", key, this.Properties[key]);
                }
            }
        }
    }
}
