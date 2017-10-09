using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FruityUI.Pages
{
    /// <summary>
    /// Interaction logic for Debugger.xaml
    /// </summary>
    ///
    

    public partial class Debugger : Page
    {
        public Debugger()
        {
            InitializeComponent();

            rtb.Document.Blocks.Clear();

            Style noSpaceStyle = new Style(typeof(Paragraph));
            noSpaceStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            rtb.Resources.Add(typeof(Paragraph), noSpaceStyle);

            rtb.Width = Width;
            rtb.Height = Height;

            rtb.TextChanged += (s, e) => update();
        }

        public void log(string a)
        {
            rtb.Document.Blocks.Add(new Paragraph(new Run(a)));
            update();
        }

        private void update()
        {
            rtb.ScrollToEnd();
        }
    }
}
