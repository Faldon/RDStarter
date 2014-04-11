using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace RDStarter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            List<string> devices = getTargetDevices();
            InitializeComponent();
            this.TargetChoice.ItemsSource = devices;
        }

        private void connectClick(Object sender, EventArgs e)
        {
            Button connectButton = (Button)sender;
            if(connectButton.Name.Equals("adconnect"))
            {
                bool requestControl = (bool)this.adcontrol.IsChecked;
                bool requestConsent = (bool)this.adconsent.IsChecked;
                string DeviceName = this.TargetChoice.SelectedItem.ToString();

                connectToDevice(DeviceName, requestControl, requestConsent);
            }
            else if (connectButton.Name.Equals("wsconnect"))
            {
                bool requestControl = (bool)this.wscontrol.IsChecked;
                bool requestConsent = (bool)this.wsconsent.IsChecked;
                string DeviceName = this.TargetDevice.Text;

                connectToDevice(DeviceName, requestControl, requestConsent);
            }
            
        }

        /*
         * Returns a List containing the dns name of all computers in the current
         * active directory running Windows 8.1 (Version 6.3 Build 9600)
         */
        private static List<string> getTargetDevices()
        {
            List<string> targetDevices = new List<string>();
            DirectorySearcher ds = new DirectorySearcher();
            ds.Filter = ("(objectClass=computer)");

            foreach(SearchResult item in ds.FindAll())
            {
                String osVersion = item.GetDirectoryEntry().Properties["operatingSystemVersion"].Value.ToString();
                if (osVersion.Equals("6.3 (9600)"))
                {
                    targetDevices.Add(item.GetDirectoryEntry().Properties["dNSHostName"].Value.ToString());
                };
            }
            return targetDevices;
        }

        /*
         * Connects to the selected computer through mstsc and shadowing
         */
        private static void connectToDevice(string DeviceName, bool requestControl, bool requestConsent)
        {
            string cmdString = "/C mstsc /shadow:1 /v:" + DeviceName;
            if (requestControl) { cmdString += " /control";  }
            if (!requestConsent) { cmdString += " /noConsentPrompt"; }

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
            si.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            si.FileName = "cmd.exe";
            si.Verb = "runas";
            si.Arguments = cmdString;
            
            p.StartInfo = si;
            p.Start();
        }
    }
}