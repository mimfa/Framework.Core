using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Controls.WinForm.Menu
{
    public class ToolStripRender : ToolStripProfessionalRenderer
    {
        public ToolStripRender() : this(new ColorTable.DefaultStripColors()) { }
        public ToolStripRender(ProfessionalColorTable pcolorTable) : base(pcolorTable) { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!(e.ToolStrip is ToolStrip)) base.OnRenderToolStripBorder(e);
        }
    }
}
