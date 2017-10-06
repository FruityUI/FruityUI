using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Reflection;
using System.Collections;

namespace FruityUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    

    public partial class MainWindow : Window
    {

        private List<string> DynamicLinkLibrary;
        private List<FruityUI.IPlugin> plugins = new List<FruityUI.IPlugin>();


        public MainWindow()
        {

            if (!string.IsNullOrEmpty(Properties.Settings.Default.dlls))
                DynamicLinkLibrary = new List<string>(Properties.Settings.Default.dlls.Split('|'));
            else DynamicLinkLibrary = new List<string>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Dynamic Link Library (IPlugin) | *.dll";
            ofd.ShowDialog();

            if (!string.IsNullOrEmpty(ofd.FileName) && ofd.CheckFileExists)
                loadLibrary(ofd.FileName);


            foreach(string dll in DynamicLinkLibrary)
            {
                Assembly a = Assembly.LoadFile(dll);

                foreach(Type i in a.GetExportedTypes())
                {

                    if (typeof(IPlugin).IsAssignableFrom(i))
                    {
                        Activator.CreateInstance(i);
                        break;
                    }
                }
            }

            Closing += save;


        }

        private void save(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.dlls = string.Join("|", DynamicLinkLibrary);
            Properties.Settings.Default.Save();
        }

        private void loadLibrary(string i)
        {

            try
            {

                Assembly a = Assembly.LoadFile(i);

                foreach(Type t in a.GetTypes())
                {
                    if (!t.IsClass) continue;
                    if (t.GetInterfaces().Contains(typeof(FruityUI.IPlugin)))
                    {
                        plugins.Add((Activator.CreateInstance(t) as FruityUI.IPlugin));
                        return;
                    }
                }

                throw new Exception("Invalid DLL, Interface not found!");

            }catch(Exception ex)
            {
                Console.WriteLine("Failed. " + ex.Message);
            }

        }
    }
}
