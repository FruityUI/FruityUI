using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FruityUI;
using System.Windows;
using System.Windows.Controls;

namespace Plugin
{
    public class Plugin : FruityUI.IPlugin
    {

        private string _name = "NewPlugin";
        private string _description = "";

        public string name { get { return _name; } }
        public string description { get { return _description;  } }

        protected static Core core;
        private Window w;

        public Plugin(Core _core)
        {
            core = _core;
            w = core.createNewWindow(_name, 200, 300, 20, 20);

            TextBox t = new TextBox();
            t.Text = "Sample";
            StackPanel p = new StackPanel();
            p.Children.Add(t);
            p.UpdateLayout();
            w.Content = p;

            Console.WriteLine("Plugin loaded");
        }

    }
}
