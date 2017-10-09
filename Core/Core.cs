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
using System.Timers;

using FruityUI.CCore.Controls;

namespace FruityUI
{
    public class Core : IDisposable
    {

        private List<FruityWindow> windows = new List<FruityWindow>();
        private Dictionary<string, dynamic> settings = new Dictionary<string, dynamic>();

        private System.Timers.Timer _onSecond = new System.Timers.Timer(1000);
        private System.Timers.Timer _onMinute = new System.Timers.Timer(1000 * 60);

        public int screen_width = Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenWidth);
        public int screen_height = Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenHeight);

        public event EventHandler<ISettings> dbUpdate;
        public event EventHandler<ElapsedEventArgs> onSecond;
        public event EventHandler<ElapsedEventArgs> onMinute;

        public Core(Dictionary<string, dynamic> _settings)
        {
            settings = _settings;
            _onSecond.Enabled = _onMinute.Enabled = true;
            _onSecond.Elapsed += onSecondEvent;
            _onMinute.Elapsed += onMinuteEvent;
            _onSecond.Start();
            _onMinute.Start();
        }

        private void onSecondEvent(Object s, ElapsedEventArgs e)
        {
            if (onSecond != null)
                onSecond(s, e);
        }

        private void onMinuteEvent(Object s, ElapsedEventArgs e)
        {
            if (onMinute != null)
                onMinute(s, e);
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

        public List<FruityWindow> getWindows()
        {
            return windows;
        }

        public Window createNewWindow(string name, int width, int height, int x = 0, int y = 0, bool hidden = false)
        {
            FruityWindow fw = new FruityWindow(name, width, height, x, y, hidden);
            windows.Add(fw);
            return fw;
        }

        ~Core()
        {
            Dispose();
        }

        public void Dispose()
        {
            _onSecond.Stop();
            GC.SuppressFinalize(this);
        }

    }
}