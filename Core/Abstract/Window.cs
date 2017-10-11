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

namespace FruityUI.CCore.Abstract
{


    public abstract class Window : System.Windows.Window, IDisposable
    {

        private SolidColorBrush background = new SolidColorBrush(Colors.Transparent);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOZORDER = 0x0004;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public Window()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            this.menu.Items.Add(close);
            this.ContextMenu = menu;
            this.Background = background;

            this.MouseUp += onMouseUp;
            close.Click += (s, e) => this.Close();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {


            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public void SendToBack(CCore.Controls.FruityWindow window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        private MenuItem close = new MenuItem()
        {
            Header = "Close"
        };

        public ContextMenu menu = new ContextMenu();

        private void onMouseUp(Object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Released && e.LeftButton != System.Windows.Input.MouseButtonState.Released)
            {
                if (menu.IsOpen) return;
                menu.IsOpen = true;
                return;
            }
            menu.IsOpen = false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
