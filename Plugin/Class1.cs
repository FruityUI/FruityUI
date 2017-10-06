using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    public class Plugin : FruityUI.IPlugin
    {

        public Plugin()
        {
            Console.WriteLine("Plugin loaded");
        }

    }
}
