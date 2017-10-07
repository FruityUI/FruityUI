using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft;
using Newtonsoft.Json;
using System.Reflection;

namespace FruityUI
{
    public class Core
    {

        private List<Window> windows = new List<Window>();
        public event EventHandler<ISettings> dbUpdate;
        private Dictionary<string, dynamic> settings = new Dictionary<string, dynamic>();

        protected static Window fui;

        public Core(Dictionary<string, dynamic> _settings)
        {
            settings = _settings;
        }

        public void updateSettings(ISettings i)
        {
            dbUpdate(this, i);
        }

        public void getSettings(ISettings i)
        {
            if (settings.ContainsKey(i.database))
            {
                dynamic data = settings[i.database];
                foreach (Newtonsoft.Json.Linq.JProperty a in data)
                {
                    Type t = i.GetType();
                    PropertyInfo prop = t.GetProperty(a.Name);
                    if (prop == null) continue;
                    if (!prop.CanWrite) continue;
                    string insert = (string)a.Value;
                    prop.SetValue(i, insert, null);
                }
            }
            /*
            if (settings.ContainsKey(i.database))
            {
                // Console.WriteLine(JsonConvert.SerializeObject(settings[i.database]));
                dynamic data = settings[i.database];
                foreach(Newtonsoft.Json.Linq.JProperty a in data)
                {
                    PropertyInfo prop = i.GetType().GetProperty(a.Name, BindingFlags.Public | BindingFlags.Instance);
                    if(prop != null && prop.CanWrite)
                    {
                        prop.SetValue(i, a.Value, null);
                    }
                }

            }
            */

        }

        public List<Window> getWindows()
        {
            return windows;
        }

        public Window createNewWindow(string name, int width, int height, int x = 0, int y = 0, bool hidden = false)
        {

            ContextMenu menu = new ContextMenu();

            MenuItem close = new MenuItem()
            {
                Header = "Close"
            };

            MenuItem info = new MenuItem()
            {
                Header = "Info"
            };

            MenuItem reload = new MenuItem()
            {
                Header = "Reload Plugin"
            };

            menu.Items.Add(close);

            Window w = new Window();

            w.MouseUp += (s, e) =>
            {
                if(e.RightButton == System.Windows.Input.MouseButtonState.Released && e.LeftButton != System.Windows.Input.MouseButtonState.Released)
                {
                    if (menu.IsOpen) return;
                    menu.IsOpen = true;
                }
                menu.IsOpen = false;
            };

            close.Click += (s, e) =>
            {
                w.Close();
            };

            w.ContextMenu = menu;
            w.AllowsTransparency = true;
            w.Title = name;
            w.ShowInTaskbar = false;
            w.Width = width;
            w.Height = height;
            w.WindowStyle = WindowStyle.None;
            w.Left = x;
            w.Top = y;
            SolidColorBrush color = new SolidColorBrush();
            color.Opacity = 0.5;
            color.Color = Colors.White;
            w.Background = color;
            w.Activate();
            if(!hidden)
                w.Show();

            windows.Add(w);
            return w;
        }

    }
}
