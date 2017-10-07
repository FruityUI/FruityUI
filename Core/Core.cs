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
using System.Runtime.InteropServices;
using System.Windows.Interop;

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
        }


        public List<Window> getWindows()
        {
            return windows;
        }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
           int Y, int cx, int cy, uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public static void SetBottom(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0,0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        private MenuItem close = new MenuItem()
        {
            Header = "Close"
        };

        private MenuItem info = new MenuItem()
        {
            Header = "Info"
        };

        private MenuItem reload = new MenuItem()
        {
            Header = "Reload Plugin"
        };

        public Window createNewWindow(string name, int width, int height, int x = 0, int y = 0, bool hidden = false)
        {

            ContextMenu menu = new ContextMenu();

            menu.Items.Add(close);

            SolidColorBrush color = new SolidColorBrush();
            color.Opacity = 0.5;
            color.Color = Colors.White;

            Window w = new Window() {
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ContextMenu = menu,
                Title = name,
                ShowInTaskbar = false,
                Width = width,
                Height = height,
                Left = x,
                Top = y,
                Background = color
            };
            w.Hide();

            SetBottom(w);

            w.MouseUp += (s, e) =>
            {
                if(e.RightButton == System.Windows.Input.MouseButtonState.Released && e.LeftButton != System.Windows.Input.MouseButtonState.Released)
                {
                    if (menu.IsOpen) return;
                    menu.IsOpen = true;
                }
                menu.IsOpen = false;
            };

            close.Click += (s, e) => w.Close();

            if (!hidden)
                w.Show();

            windows.Add(w);
            return w;
        }

    }

}
