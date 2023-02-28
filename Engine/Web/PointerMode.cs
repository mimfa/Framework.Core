using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Web
{
    public enum PointerMode
    {
        Pure = -1,
        Id = 0,
        Name = 1,
        Tag = 2,
        Class = 3,
        Location = 4,
        Query = 5,
        XPath = 6
    }
}
