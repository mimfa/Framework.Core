using MiMFa.Engine.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Controls.WinForm.Menu.ColorTable
{
    public class DefaultStripColors : PaletteStripColors
    {
        public override IPalette Palette
        {
            get => Default.HasTemplator?Default.Templator.Palette:new PaletteBase();
            set
            {
                if (Default.HasTemplator && value != null) Default.Templator.Palette = value;
            }
        }

        public DefaultStripColors() : base(null)
        {
        }

    }
}
