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
        private OpenFileDialog ofd;
        private FruityUI.Core core;
        private ToolBar tb = new ToolBar();
        private List<ISettings> settings = new List<ISettings>();
        private Dictionary<string, dynamic> setkeys = new Dictionary<string, dynamic>();

        public MainWindow()
        {
            InitializeComponent();

            if(!string.IsNullOrEmpty(Properties.Settings.Default.settings))
            {
                // todo: Restore settings 
                dynamic p = JsonConvert.DeserializeObject<dynamic>(Properties.Settings.Default.settings);
                foreach(dynamic o in p)
                {
                    object data = o.data;
                    string name = o.database;
                    setkeys.Add(name, data);
                }

            }


            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = true;
            core = new FruityUI.Core(setkeys);

            core.dbUpdate += (s, e) =>
            {
                int i = settings.FindIndex(c => c.database == e.database);
                if(i > -1)
                {
                    Console.WriteLine("#Update " + settings[i].database + " > " + e.database);
                    settings[i] = e;
                }else
                {
                    Console.WriteLine("New database added " + e.database);
                    settings.Add(e);
                }
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



        }

        private void getLibrary()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "(IPlugin) | *.dll";
            ofd.ShowDialog();
            if(!string.IsNullOrEmpty(ofd.FileName) && ofd.CheckFileExists)
                loadLibrary(ofd.FileName);
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
            windows.Clear();
            Environment.Exit(0);
        }


        private void loadLibrary(string i)
        {

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
