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

using FruityUI.CCore.Controls;
using System.Windows.Media.Animation;

namespace FruityUI
{

    public partial class MainWindow : Window
    {

        private List<string> DynamicLinkLibrary = new List<string>();
        private List<string> loadedLibraries = new List<string>();
        private List<IPlugin> plugins = new List<IPlugin>();
        private Core core;
        private Dictionary<string, dynamic> settings = new Dictionary<string, dynamic>();
        private bool up4reset = false;

        private Pages.Debugger debugger_page;
        private Pages.Installer installer_page;
        private Pages.Plugins plugins_page;
        private Pages.Settings settings_page;
        private Menu m;

        FruityUI.DBGConsole console = new DBGConsole();

        public MainWindow(SplashScreen ss)
        {
            InitializeComponent();

            DateTime start = DateTime.Now;

            debugger_page = new Pages.Debugger();

            if (!console.isDebugging)
            {
                Console.SetOut(console);
                console.output += (s, e) => debugger_page.log(e);
            }
            else debugger_page.log("FruityUI is in debug mode. Logging is shown in the console.");

            if (Properties.Settings.Default.settings != null && Properties.Settings.Default.settings != String.Empty)
            {
                if (Properties.Settings.Default.settings.Length > 2)
                {
                    dynamic d = JsonConvert.DeserializeObject(Properties.Settings.Default.settings);
                    foreach (var i in d)
                        settings.Add(i.Key as string, JsonConvert.DeserializeObject(i.Value as string) as dynamic);
                }
            }

            core = new FruityUI.Core(settings);

            core.dbUpdate += (s, e) =>
            {
                if (settings.ContainsKey(e.database)) settings[e.database] = JsonConvert.SerializeObject(e);
                else settings.Add(e.database, JsonConvert.SerializeObject(e));
                save();
            };

            if (Properties.Settings.Default.dlls != null && Properties.Settings.Default.dlls != String.Empty)
                DynamicLinkLibrary = new List<string>(Properties.Settings.Default.dlls.Split('|'));

            if(DynamicLinkLibrary.Count() >= 1)
                LoadLibraries();

            Closing += terminate;

            installer_page = new Pages.Installer(this);
            plugins_page = new Pages.Plugins();
            settings_page = new Pages.Settings();

            m = new Menu(mButton, xMenu, updateScreen);

            SizeChanged += (s, e) => updateScreen();

            frame.Content = installer_page;

            install_plugins_btn.Click += (s, e) => frame.Content = installer_page;
            debug_btn.Click += (s, e) => frame.Content = debugger_page;
            plugins_btn.Click += (s, e) => frame.Content = plugins_page;
            settings_btn.Click += (s, e) => frame.Content = settings_page;

            closeMenu.Click += (s, e) =>
            {
                m.closeMenu();
                new Thread(() =>
                {
                    Thread.Sleep(500);
                    Application.Current.Dispatcher.Invoke(() => { updateScreen(); });
                }).Start(); ;
            };

            Console.WriteLine("FruityUI loaded in " + (DateTime.Now - start).TotalSeconds + "s");
            ss.Close();
        }

        private void updateScreen()
        {
            double x = (Width - 10);
            x -= (m.IsOpen) ? xMenu.Width : (xMenu.Width / 2);
            frame.Width = x;
        }

        public void set4Reset()
        {
            up4reset = true;
            Properties.Settings.Default.settings = "[]";
            Properties.Settings.Default.dlls = String.Empty;
            Properties.Settings.Default.Save();
        }

        public void getLibrary()
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
            if (!up4reset)
            {
                Properties.Settings.Default.dlls = string.Join("|", DynamicLinkLibrary);
                Properties.Settings.Default.settings = JsonConvert.SerializeObject(settings.ToList());
            }
            Properties.Settings.Default.Save();
        }

        private void terminate(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            save();
            List<FruityWindow> windows = core.getWindows();
            foreach (FruityWindow w in windows)
                w.Close();
            foreach(IPlugin plugin in plugins)
                plugin.Dispose();
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
                    if (!t.IsClass || t.IsNotPublic) continue;
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
                            DynamicLinkLibrary.Add(i);
                            loadedLibraries.Add(i);
                            Console.WriteLine("Plugin <"+t.Name+"> loaded.");
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Error occured within the plugin <" + t.Name + ">. " + ex.Message);
                            Console.WriteLine("Error occured within the plugin <" + t.Name + ">. " + ex.Message);
                            return;
                        }
                        return;
                    }else
                    {
                        if (a.GetTypes().Last() != t)
                            continue;
                        MessageBox.Show("Plugin loaded does not extend IPlugin");
                        Console.WriteLine("Plugin loaded does not extend IPlugin");
                        return;
                    }
                }

                MessageBox.Show("Invalid dynamic link library @ " + i);
                Console.WriteLine("Invalid dynamic link library @ " + i);

            }catch(Exception ex)
            {
                MessageBox.Show("Failed to load plugin. " + ex.Message);
                Console.WriteLine("Failed. " + ex.Message);
            }

        }
    }
}
