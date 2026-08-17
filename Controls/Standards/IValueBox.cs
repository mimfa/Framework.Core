using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Controls.Standards
{
    public interface IValueBox<T> : IStandardControl
    {
        event EventHandler ValueChanged;
        T Value { get; set; }
    }
}
