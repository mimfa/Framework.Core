using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Engine.Template
{
    public interface ITemplatable
    {
        void Template(Control control);
    }
}
