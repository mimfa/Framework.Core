using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Exclusive.ProgramingTechnology.CommandLanguage
{
    [Serializable]
    public delegate object MRLEventHandler(HtmlElement element, EventArgs eventName, params object[] handlerValue);
    public delegate object MRLEventHandlerP(IHTMLElement element, IHTMLEventObj eventName, params object[] handlerValue);
}
