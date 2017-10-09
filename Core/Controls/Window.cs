using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace FruityUI.CCore.Controls
{
    public class FruityWindow : Abstract.Window, IDisposable
    {

        private SolidColorBrush background = new SolidColorBrush(Colors.Transparent);

        public FruityWindow(string name, int width, int height, int x = 0, int y = 0, bool hide = false) : base() {

            Hide();
            menu.Items.Add(close);
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            ContextMenu = menu;
            Title = name;
            ShowInTaskbar = false;
            Width = width;
            Height = height;
            Left = x;
            Top = y;
            Background = background;
            SendToBack(this);
            this.MouseUp += onMouseUp;
            close.Click += (s, e) => Close();
            if (!hide)
                Show();
        }

        private void onMouseUp(Object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Released && e.LeftButton != System.Windows.Input.MouseButtonState.Released)
            {
                if (menu.IsOpen) return;
                menu.IsOpen = true;
            }
            menu.IsOpen = false;
        }

        public void Invoke(Action a)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            { a(); });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
