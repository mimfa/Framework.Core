using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Controls.Standards
{
    public interface ITextBox : IStandardControl
    {
        event EventHandler TextChanged;
        string Text { get; set; }
    }
}
