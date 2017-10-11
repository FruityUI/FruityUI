using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Deployment;
using System.Xml;
using System.Reflection;

namespace FruityUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashScreen ss = new SplashScreen(getVersion());
            ss.Show();

            if ((Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)).Length >= 2)
            {
                MessageBox.Show("Cannot run 2 instances of FruityUI");
                ss.Close();
                Environment.Exit(0);
            }

            FruityUI.MainWindow w = new FruityUI.MainWindow(ss);
            w.Show();

        }

        private Version getVersion()
        {

            XmlDocument xmlDoc = new XmlDocument();
            Assembly asnCurrent = System.Reflection.Assembly.GetExecutingAssembly();
            string executePath = new Uri(asnCurrent.GetName().CodeBase).LocalPath;

            xmlDoc.Load(executePath + ".manifest");
            string retval = string.Empty;

            if (xmlDoc.HasChildNodes)
                retval = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes.GetNamedItem("version").Value.ToString();

            return new Version(retval);
        }


    }
}
