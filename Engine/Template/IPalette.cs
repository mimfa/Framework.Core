using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Template
{
    public interface IPalette
    {
        Font Font { get; set; }
        Font MenuFont { get; set; }
        Font LabelFont { get; set; }
        Font InputFont { get; set; }
        Font ButtonFont { get; set; }
        Font SpecialFont { get; set; }
        Font BigFont { get; set; }
        Font SmallFont { get; set; }

        Color BackColor { get; set; }
        Color ForeColor { get; set; }

        Color MenuBackColor { get; set; }
        Color LabelBackColor { get; set; }
        Color InputBackColor { get; set; }
        Color ButtonBackColor { get; set; }
        Color SpecialBackColor { get; set; }

        Color MenuForeColor { get; set; }
        Color LabelForeColor { get; set; }
        Color InputForeColor { get; set; }
        Color ButtonForeColor { get; set; }
        Color SpecialForeColor { get; set; }

        Color FirstSpecialBackColor { get; set; }
        Color SecondSpecialBackColor { get; set; }
        Color ThirdSpecialBackColor { get; set; }
        Color FirstSpecialForeColor { get; set; }
        Color SecondSpecialForeColor { get; set; }
        Color ThirdSpecialForeColor { get; set; }

        Font UpdateFont(string path);
        Font UpdateFont(FontFamily fontFamily);
        Font UpdateFont(Font font);
    }
}
