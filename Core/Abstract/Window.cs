using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace FruityUI.CCore.Abstract
{

    // Keep Window Back
    struct KWB_FLAGS
    {
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 SWP_NOACTIVATE = 0x0010;
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }

    public abstract class Window : System.Windows.Window
    {


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

        public void SendToBack(CCore.Controls.FruityWindow window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            KWB_FLAGS.SetWindowPos(hWnd, KWB_FLAGS.HWND_BOTTOM, 0, 0, 0, 0, KWB_FLAGS.SWP_NOSIZE | KWB_FLAGS.SWP_NOMOVE | KWB_FLAGS.SWP_NOACTIVATE);
        }

        public MenuItem close = new MenuItem()
        {
            Header = "Close"
        };

        public ContextMenu menu = new ContextMenu();

    }
}
