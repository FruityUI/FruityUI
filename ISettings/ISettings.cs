using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruityUI
{

    public interface ISettings
    {
        string database { get; }
        dynamic data { get; }
    }

}
