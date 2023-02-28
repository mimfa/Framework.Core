using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Controls.WinForm.Menu.ColorTable
{
    public class BrowserStripColors : ProfessionalColorTable
    {
        public override Color ToolStripBorder => Color.Transparent;
        public override Color ToolStripGradientBegin => Color.Transparent;
        public override Color ToolStripGradientMiddle => Color.Transparent;
        public override Color ToolStripGradientEnd => Color.Transparent;
        public override Color ToolStripContentPanelGradientBegin => Color.Transparent;
        public override Color ToolStripContentPanelGradientEnd => Color.Transparent;
        public override Color ToolStripDropDownBackground => Color.Transparent;
        public override Color ToolStripPanelGradientBegin => Color.Transparent;
        public override Color ToolStripPanelGradientEnd => Color.Transparent;

        public override Color MenuItemSelected => Color.FromArgb(100,100,100,100); 
        public override Color MenuStripGradientBegin => Color.Transparent;
        public override Color MenuStripGradientEnd => Color.Transparent;
        public override Color RaftingContainerGradientBegin => Color.Transparent;
        public override Color RaftingContainerGradientEnd => Color.Transparent;

        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(100, 100, 100, 100); 
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(30, 30, 30);

        public override Color MenuBorder => Color.FromArgb(100, 100, 100, 100);
        public override Color MenuItemBorder => Color.FromArgb(100, 100, 100, 100);

        public override Color SeparatorLight => base.SeparatorLight;
        public override Color SeparatorDark => base.SeparatorDark;

        public override Color GripLight => Color.Transparent;
        public override Color GripDark => Color.Transparent;


        public override Color ButtonCheckedGradientBegin => Color.Transparent;
        public override Color ButtonCheckedGradientEnd => Color.Transparent;
        public override Color ButtonCheckedGradientMiddle => Color.Transparent;
        public override Color ButtonCheckedHighlight => Color.Transparent;
        public override Color ButtonCheckedHighlightBorder => Color.Transparent;
        public override Color ButtonPressedBorder => Color.Transparent;
        public override Color ButtonPressedGradientBegin => Color.Transparent;
        public override Color ButtonPressedGradientEnd => Color.Transparent;
        public override Color ButtonPressedGradientMiddle => Color.Transparent;
        public override Color ButtonPressedHighlight => Color.Transparent;
        public override Color ButtonPressedHighlightBorder => Color.Transparent;
        public override Color ButtonSelectedBorder => Color.Transparent;
        public override Color ButtonSelectedGradientBegin => Color.Transparent;
        public override Color ButtonSelectedGradientEnd => Color.Transparent;
        public override Color ButtonSelectedGradientMiddle => Color.Transparent;
        public override Color ButtonSelectedHighlight => Color.Transparent;
        public override Color ButtonSelectedHighlightBorder => Color.Transparent;
        public override Color CheckBackground => Color.Transparent;
        public override Color CheckPressedBackground => Color.Transparent;
        public override Color CheckSelectedBackground => Color.Transparent;
        public override Color ImageMarginGradientBegin => Color.Transparent;
        public override Color ImageMarginGradientEnd => Color.Transparent;
        public override Color ImageMarginGradientMiddle => Color.Transparent;
        public override Color ImageMarginRevealedGradientBegin => Color.Transparent;
        public override Color ImageMarginRevealedGradientEnd => Color.Transparent;
        public override Color ImageMarginRevealedGradientMiddle => Color.Transparent;
        public override Color MenuItemPressedGradientBegin => Color.Transparent;
        public override Color MenuItemPressedGradientEnd => Color.Transparent;
        public override Color MenuItemPressedGradientMiddle => Color.Transparent;
        public override Color OverflowButtonGradientBegin => Color.Transparent;
        public override Color OverflowButtonGradientEnd => Color.Transparent;
        public override Color OverflowButtonGradientMiddle => Color.Transparent;
        public override Color StatusStripGradientBegin => Color.Transparent;
        public override Color StatusStripGradientEnd => Color.Transparent;
    }
}
