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
    public class PaletteStripColors : ProfessionalColorTable
    {
        public virtual IPalette Palette { get; set; }
        public PaletteStripColors(IPalette palette)
        {
            Palette = palette;
        }

        public override Color MenuBorder => Palette.MenuBackColor;
        public override Color MenuStripGradientBegin =>  Palette.MenuBackColor;
        public override Color MenuStripGradientEnd =>  Palette.MenuBackColor;

        public override Color ButtonSelectedBorder =>  Palette.MenuForeColor;
        public override Color ButtonSelectedHighlightBorder =>  Palette.ButtonForeColor;
        public override Color ButtonSelectedHighlight =>  Palette.SpecialBackColor;
        public override Color ButtonSelectedGradientBegin => Color.FromArgb(100,  Palette.SpecialBackColor);
        public override Color ButtonSelectedGradientMiddle => Color.FromArgb(100,  Palette.SpecialBackColor);
        public override Color ButtonSelectedGradientEnd => Color.FromArgb(100,  Palette.SpecialBackColor);

        public override Color ButtonPressedBorder =>  Palette.MenuForeColor;
        public override Color ButtonPressedHighlightBorder => Palette.FirstSpecialBackColor;
        public override Color ButtonPressedHighlight =>  Palette.MenuBackColor;
        public override Color ButtonPressedGradientBegin =>  Palette.MenuBackColor;
        public override Color ButtonPressedGradientMiddle =>  Palette.ButtonBackColor;
        public override Color ButtonPressedGradientEnd =>  Palette.MenuBackColor;

        public override Color ButtonCheckedHighlight =>  Palette.MenuBackColor;
        public override Color ButtonCheckedHighlightBorder =>  Palette.ButtonForeColor;
        public override Color ButtonCheckedGradientBegin =>  Palette.ButtonBackColor;
        public override Color ButtonCheckedGradientMiddle =>  Palette.ButtonBackColor;
        public override Color ButtonCheckedGradientEnd =>  Palette.ButtonBackColor;

        public override Color CheckBackground => Palette.FirstSpecialBackColor;
        public override Color CheckSelectedBackground =>  Palette.ButtonForeColor;
        public override Color CheckPressedBackground =>  Palette.MenuForeColor;

        public override Color ImageMarginGradientBegin =>  Palette.MenuBackColor;
        public override Color ImageMarginGradientMiddle =>  Palette.MenuBackColor;
        public override Color ImageMarginGradientEnd =>  Palette.MenuBackColor;
        public override Color ImageMarginRevealedGradientBegin => ImageMarginGradientBegin;
        public override Color ImageMarginRevealedGradientMiddle => ImageMarginGradientMiddle;
        public override Color ImageMarginRevealedGradientEnd => ImageMarginGradientEnd;

        public override Color SeparatorDark =>  Palette.MenuBackColor;
        public override Color SeparatorLight =>  Palette.MenuForeColor;

        public override Color MenuItemBorder =>  Palette.ButtonBackColor;

        public override Color MenuItemSelected => Palette.SpecialBackColor;
        public override Color MenuItemSelectedGradientBegin =>  Palette.SpecialBackColor;
        public override Color MenuItemSelectedGradientEnd => Palette.SpecialBackColor;

        public override Color MenuItemPressedGradientBegin =>  Palette.ButtonBackColor;
        public override Color MenuItemPressedGradientMiddle =>  Palette.ButtonBackColor;
        public override Color MenuItemPressedGradientEnd =>  Palette.ButtonBackColor;

        public override Color RaftingContainerGradientBegin => MenuStripGradientBegin;
        public override Color RaftingContainerGradientEnd => MenuStripGradientEnd;

        public override Color StatusStripGradientBegin => MenuStripGradientBegin;
        public override Color StatusStripGradientEnd => MenuStripGradientEnd;

        public override Color ToolStripBorder => MenuBorder;

        public override Color ToolStripGradientBegin => MenuStripGradientBegin;
        public override Color ToolStripGradientMiddle => MenuStripGradientEnd;
        public override Color ToolStripGradientEnd => MenuStripGradientBegin;

        public override Color ToolStripPanelGradientBegin => MenuStripGradientBegin;
        public override Color ToolStripPanelGradientEnd => MenuStripGradientEnd;

        public override Color ToolStripContentPanelGradientBegin => MenuStripGradientBegin;
        public override Color ToolStripContentPanelGradientEnd => MenuStripGradientEnd;

        public override Color ToolStripDropDownBackground => MenuStripGradientEnd;

        public override Color OverflowButtonGradientBegin => ButtonSelectedGradientBegin;
        public override Color OverflowButtonGradientMiddle => ButtonSelectedGradientMiddle;
        public override Color OverflowButtonGradientEnd => ButtonSelectedGradientEnd;
    }
}
