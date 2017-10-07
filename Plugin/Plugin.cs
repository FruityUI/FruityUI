using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FruityUI;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Plugin
{

    class Settings : ISettings
    {

        private string _database = "anExamplePlugin";
        private dynamic _data;

        public string database {  get { return _database; } }
        public dynamic data { get { return _data;  } }

        public Settings(dynamic d = null)
        {
            if(d != null)
                _data = d;
        }


    }

    public class Plugin : FruityUI.IPlugin
    {

        private string _name = "anExamplePlugin";
        private string _description = "";

        public string name { get { return _name; } }
        public string description { get { return _description;  } }

        private Settings settings = new Settings();

        protected static Core core;
        private Window w;

        public Plugin(Core _core)
        {
            core = _core;
            w = core.createNewWindow(_name, 200, 300, 20, 20);

            dynamic data = core.getSettings("anExamplePlugin");
            if (data != null)
            {
                settings = new Settings(data);
            }else
            {
                settings = new Settings();
            }

            core.updateSettings(settings);

            TextBox t = new TextBox();
            t.Text = "Sample";
            StackPanel p = new StackPanel();
            p.Children.Add(t);
            p.UpdateLayout();
            w.Content = p;

            Console.WriteLine("Plugin loaded");
        }

        public ISettings getSettings()
        {
            return settings;
        }

    }
}
