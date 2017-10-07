using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                Console.WriteLine("clicked");
                if(e.RightButton == System.Windows.Input.MouseButtonState.Released && e.LeftButton != System.Windows.Input.MouseButtonState.Released)
                {
                    if (menu.IsOpen)
                    {
                        menu.IsOpen = false;
                        return;
                    }
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
            return w;
        }

    }
}
