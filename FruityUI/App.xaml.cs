using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FruityUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            SplashScreen ss = new SplashScreen();
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


    }
}
