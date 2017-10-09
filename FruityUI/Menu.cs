using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FruityUI
{
    class Menu
    {

        private bool IsOpen = false;


        protected static Rectangle rec;
        protected static Viewbox view;

        public Menu(Rectangle _rec, Viewbox _view)
        {

            rec = _rec;
            view = _view;

            view.Opacity = 0.0;

            rec.MouseEnter += (s, e) =>
            {
                if (!IsOpen)
                {
                    openMenu();
                }
            };

        }

        public void openMenu()
        {
            if (IsOpen) return;
            var anim = new DoubleAnimation(0.9, (Duration)TimeSpan.FromSeconds(0.5));
            view.BeginAnimation(UIElement.OpacityProperty, anim);
            IsOpen = true;
        }

        public void closeMenu()
        {
            if (!IsOpen) return;
            var anim = new DoubleAnimation(0.0, (Duration)TimeSpan.FromSeconds(0.5));
            view.BeginAnimation(UIElement.OpacityProperty, anim);
            IsOpen = false;
        }

    }
}
