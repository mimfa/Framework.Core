using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Engine.Web
{
    public interface IPointerJSSupported
    {
        PointerJS GetPointerJS();
        PointerJS GetPointerJS(string pointer, PointerMode pointerType);
        PointerJS GetPointerJS(long x, long y);
        PointerJS GetPointerJS(string query);
    }
}
