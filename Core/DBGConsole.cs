using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruityUI
{
    public class DBGConsole : TextWriter
    {

        private TextWriter originalOut = Console.Out;

        private bool _isDebugging = false;
        public bool isDebugging { get { return _isDebugging; } }

        public override Encoding Encoding
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public event EventHandler<string> output;

        public DBGConsole()
        {
            _isDebugging = System.Diagnostics.Debugger.IsAttached;
        }

        public override void WriteLine(string a)
        {
            if (_isDebugging)
            {
                originalOut.WriteLine(a);
            }
            else
            {
                if (output != null)
                    output(this, a);
            }

        }

    }
}
