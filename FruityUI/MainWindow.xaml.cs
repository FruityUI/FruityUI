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
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;

namespace FruityUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {

        private List<string> DynamicLinkLibrary = new List<string>();
        private List<string> loadedLibraries = new List<string>();
        private List<FruityUI.IPlugin> plugins = new List<FruityUI.IPlugin>();
        private FruityUI.Core core;
        private Dictionary<string, dynamic> settings = new Dictionary<string, dynamic>();

        public MainWindow()
        {
            InitializeComponent();

            if(!string.IsNullOrEmpty(Properties.Settings.Default.settings))
            {
                if (Properties.Settings.Default.settings.Length > 2)
                {
                    dynamic d = JsonConvert.DeserializeObject(Properties.Settings.Default.settings);
                    foreach(var i in d)
                        settings.Add((string)i.Key, JsonConvert.DeserializeObject((string)i.Value));
                }
            }


            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = true;
            core = new FruityUI.Core(settings);

            core.dbUpdate += (s, e) =>
            {

                if (settings.ContainsKey(e.database)) settings[e.database] = JsonConvert.SerializeObject(e);
                else settings.Add(e.database, JsonConvert.SerializeObject(e));
                save();
            };

            if (!string.IsNullOrEmpty(Properties.Settings.Default.dlls))
                DynamicLinkLibrary = new List<string>(Properties.Settings.Default.dlls.Split('|'));

            if(DynamicLinkLibrary.Count() >= 1)
                LoadLibraries();

            Closing += terminate;

            button.Click += (s, e) =>
            {
                getLibrary();
            };

            button1.Click += (s, e) =>
            {
                Properties.Settings.Default.settings = "[]";
                Properties.Settings.Default.dlls = "";
                Properties.Settings.Default.Save();
            };

        }

        private void getLibrary()
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "(IPlugin) | *.dll";
            ofd.ShowDialog();
            if(!string.IsNullOrEmpty(ofd.FileName) && ofd.CheckFileExists)
            {
                loadLibrary(ofd.FileName);
                return;
            }
            MessageBox.Show("No DLL file was loaded");
        }

        private void LoadLibraries()
        {
            foreach(string dll in DynamicLinkLibrary.ToList())
                loadLibrary(dll);
        }


        private void save()
        {
            // remove duplicates
            DynamicLinkLibrary = DynamicLinkLibrary.Distinct().ToList();
            Properties.Settings.Default.dlls = string.Join("|", DynamicLinkLibrary);
            Properties.Settings.Default.settings = JsonConvert.SerializeObject(settings.ToList());
            Properties.Settings.Default.Save();
        }

        private void terminate(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            save();
            List<Window> windows = core.getWindows();
            foreach (Window w in windows)
                w.Close();
            foreach(IPlugin plugin in plugins)
            {
                plugin.Dispose();
            }
            plugins.Clear();
            windows.Clear();
            Environment.Exit(0);
        }


        private void loadLibrary(string i)
        {
            // todo, use AppDomain to be able to dispose/reload plugins
            try
            {
                Assembly a = Assembly.LoadFile(i);

                foreach(Type t in a.GetTypes())
                {
                    if (!t.IsClass && t.IsNotPublic) continue;
                    if (plugins.IndexOf(t as FruityUI.IPlugin) > -1 || loadedLibraries.IndexOf(i) > -1)
                    {
                        MessageBox.Show("Duplicate plugin found. Ignored loading another instance.");
                        return;
                    }
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
                        loadedLibraries.Add(i);
                        Console.WriteLine("Plugin <{0}> loaded.", t.Name);
                        return;
                    }else
                    {
                        if (a.GetTypes().Last() != t)
                            continue;
                        MessageBox.Show("Plugin loaded does not extend IPlugin");
                        return;
                    }
                }

                MessageBox.Show("Invalid dynamic link library @ " + i);

            }catch(Exception ex)
            {
                Console.WriteLine("Failed. " + ex.Message);
            }

        }
    }
}
