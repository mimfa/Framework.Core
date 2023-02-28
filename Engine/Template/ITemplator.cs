using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MiMFa.Engine.Template
{
    public interface ITemplator
    {
        IPalette Palette { get; set; }
        Control MainControl { get; set; }
        bool OnColors { get; set; }
        bool OnFonts { get; set; }
        bool OnTabIndices { get; set; }

        ITemplator Update(Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls);
       
        Font Get(Font oldFont, Font newFont);
        Color Get(Color oldColor, Color newColor);
        int Get(int oldTabIndex, int newTabIndex);
        bool IsInherit(Color oldColor);

        int CreateTabIndex(Control forControl);
    }
}
