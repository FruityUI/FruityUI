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
    public class FruityWindow : Abstract.Window
    {

        private SolidColorBrush background = new SolidColorBrush(Colors.Transparent);

        public FruityWindow(string name, int width, int height, int x = 0, int y = 0, bool hide = false) : base() {

            Title = name;
            Width = width;
            Height = height;
            Left = x;
            Top = y;
            Background = background;
            SendToBack(this);
            if (!hide)
                Show();
        }

        public void Invoke(Action a)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            { a(); });
        }

    }
}
