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

        private List<string> DynamicLinkLibrary = new List<string>();
        private List<FruityUI.IPlugin> plugins = new List<FruityUI.IPlugin>();
        private OpenFileDialog ofd;
        private FruityUI.Core core;
        private ToolBar tb;

        public MainWindow()
        {
            this.Hide();
            this.ShowInTaskbar = true;
            core = new FruityUI.Core(this);

            if (!string.IsNullOrEmpty(Properties.Settings.Default.dlls))
                DynamicLinkLibrary = new List<string>(Properties.Settings.Default.dlls.Split('|'));

            if(DynamicLinkLibrary.Count() >= 1)
                LoadLibraries();

            Closing += save;


        }

        public void ehh() { }

        private void getLibrary()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "(IPlugin) | *.dll";
            if(!string.IsNullOrEmpty(ofd.FileName) && ofd.CheckFileExists)
            {
                loadLibrary(ofd.FileName);
            }
        }

        private void LoadLibraries()
        {
            foreach(string dll in DynamicLinkLibrary.ToList())
                loadLibrary(dll);
        }

        private void save(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            // remove duplicates
            DynamicLinkLibrary = DynamicLinkLibrary.Distinct().ToList();
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
                    if(plugins.IndexOf(t as FruityUI.IPlugin) > -1)
                    {
                        Console.WriteLine("Duplicate plugin found. Could not load");
                        continue;
                    }
                    if (!t.IsClass) continue;
                    if (t.GetInterfaces().Contains(typeof(FruityUI.IPlugin)))
                    {
                        try
                        {
                            plugins.Add((Activator.CreateInstance(t, core) as FruityUI.IPlugin));
                        }catch(Exception ex)
                        {
                            MessageBox.Show("Error occured within the plugin '" + t.Assembly.FullName + "'. " + ex.Message);
                            return;
                        }
                        DynamicLinkLibrary.Add(i);
                        Console.WriteLine("Plugin <{0}> loaded.", t.Name);
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
