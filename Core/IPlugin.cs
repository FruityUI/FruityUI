using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruityUI
{
    public interface IPlugin : IDisposable
    {
        string name { get; }
        string author { get; }
        string description { get; }
        new void Dispose();
    }
}
