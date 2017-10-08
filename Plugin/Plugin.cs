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
using System.Windows.Documents;

namespace Plugin
{

    class Settings : ISettings
    {

        private string _database;
        public string database { get { return _database; } }

        public string savedText { get; set; }

        public Settings(string name)
        {
            _database = name;
        }

    }

    public class StickyNote : FruityUI.IPlugin, IDisposable
    {

        private string _name = "StickyNote";
        private string _author = "LegitSoulja";
        private string _description = "Keep track of your notes";

        public string name { get { return _name; } }
        public string description { get { return _description;  } }
        public string author { get { return _author; } }


        private Settings settings;

        protected static Core core;
        private Window w;

        public StickyNote(Core _core)
        {
            try
            {
                core = _core;
                settings = new Settings(_name);
                core.getSettings(settings); 
                core.updateSettings(settings);
                w = core.createNewWindow(_name, 200, 300, core.screen_width - 200, 0);
                StackPanel p = new StackPanel();

                RichTextBox tb = new RichTextBox();

                tb.Document.Blocks.Clear();

                Style noSpaceStyle = new Style(typeof(Paragraph));
                noSpaceStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
                tb.Resources.Add(typeof(Paragraph), noSpaceStyle);


                tb.Height = w.Height;
                tb.Width = w.Width;

                p.Children.Add(tb);

                if (!string.IsNullOrEmpty(settings.savedText))
                    tb.Document.Blocks.Add(new Paragraph(new Run(settings.savedText)));

                tb.TextChanged += (s, e) =>
                {
                    TextRange a = new TextRange(tb.Document.ContentStart, tb.Document.ContentEnd);
                    settings.savedText = a.Text.ToString();
                };

                p.UpdateLayout();
                w.Content = p;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ~StickyNote()
        {
            Dispose();
        }

        public void Dispose()
        {
            core.updateSettings(settings);
            GC.SuppressFinalize(this);
        }


    }
}
