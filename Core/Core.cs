using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FruityUI
{
    public class Core
    {

        protected static FruityUI.MainWindow window;

        public Core(FruityUI.MainWindow w)
        {
            window = w;
        }

        public Window createNewWindow(string name, int x, int y, bool hidden = false)
        {
            Window w = new Window();
            w.Title = name;
            w.ShowInTaskbar = true;
            w.BeginInit();
            w.Width = x;
            w.Height = y;
            w.BorderThickness = new Thickness(0);
            w.Background = new SolidColorBrush(Colors.Transparent);
            w.Activate();
            if(!hidden)
                w.Show();
            return w;
        }

    }
}
