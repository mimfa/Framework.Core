using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model.Status
{
    public interface IStatus
    {
        bool IsVeryNegative { get; set; }
        bool IsNegative { get; set; }
        bool IsMiddle { get; set; }
        bool IsPositive { get; set; }
        bool IsVeryPositive { get; set; }
        bool IsVeryUseful { get; set; }
        bool IsUseful { get; set; }
        bool IsStable { get; set; }
        bool IsRisky { get; set; }
        bool IsHarmful { get; set; }
        bool IsVeryHarmful { get; set; }
    }
}
