using MiMFa.General;
using MiMFa.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace MiMFa.Exclusive.View
{
    public class ToText : ToStringBase
    {
        public override bool Translate { get; set; } = false;
        public override string StartSign { get; set; } = "";
        public override string MiddleSign { get; set; } = "->";
        public override string EndSign { get; set; } = "";
        public override string SplitSign { get; set; } = ", ";
        public override string TabSign { get; set; } = "\t ";
        public override string BreakSign { get; set; } = Environment.NewLine;
        public override string BreakLineSign { get; set; } = Environment.NewLine+ "---------------------------------------------------" + Environment.NewLine;
        public override string Table_ { get; set; } = Environment.NewLine + "___________________________________________________" + Environment.NewLine;
        public override string TableRow_ { get; set; } = "";
        public override string TableCell_ { get; set; } = "";
        public override string _TableCell { get; set; } = "\t\t ";
        public override string _TableRow { get; set; } = Environment.NewLine;
        public override string _Table { get; set; } = Environment.NewLine + "_________________________________" + Environment.NewLine;

        public override String Done(Bitmap arg)
        {
            string address = TemporaryDirectory + DateTime.Now.Ticks + ".jpg";
            arg.Save(address);
            System.Diagnostics.Process.Start(address);
            return null;
        }
    }
}
