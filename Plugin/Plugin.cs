using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FruityUI;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Reflection;

namespace Plugin
{

    class Settings : ISettings
    {

        private string _database;
        public string database { get { return _database; } }

        public Settings(string name)
        {
            _database = name;
        }

    }

    public class Plugin : FruityUI.IPlugin
    {

        private string _name = "ExamplePlugin";
        private string _author = "LegitSoulja";
        private string _description = "";

        public string name { get { return _name; } }
        public string description { get { return _description;  } }
        public string author { get { return _author; } }


        private Settings settings;

        protected static Core core;
        private Window w;

        public Plugin(Core _core)
        {
            core = _core;
            settings = new Settings(_name);
            core.getSettings(settings); // fill fields from database
            core.updateSettings(settings); // update settings (If any changes was made)
            w = core.createNewWindow(_name, 200, 300, 20, 20);

            TextBox t = new TextBox();
            t.Text = "Sample";
            StackPanel p = new StackPanel();
            p.Children.Add(t);
            p.UpdateLayout();
            w.Content = p;
        }


    }
}
