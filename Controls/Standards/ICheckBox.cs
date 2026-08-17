using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Controls.Standards
{
    public interface ICheckBox:IStandardControl
    {
        event EventHandler CheckedChanged;
        bool Checked { get; set; }
    }
}
