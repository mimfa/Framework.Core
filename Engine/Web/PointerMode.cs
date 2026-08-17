using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Web
{
    public enum PointerMode
    {
        Undefined = -1,
        Pure = 0,
        Id = 1,
        Name = 2,
        Tag = 3,
        Class = 4,
        Location = 5,
        Regex = 6,
        Query = 7,
        XPath = 8
    }
}
