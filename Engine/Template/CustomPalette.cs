using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Template
{
    public class CustomPalette : PaletteBase
    {
        public CustomPalette(Color backColor, Color foreColor, Font font = null, Color? specialBackColor = null, Color? specialForeColor = null, Font specialFont = null)
        {
            UpdateFont(font);
            SpecialFont = specialFont?? SpecialFont;
            MiMFa.Graphic.ProcessColor pi = new Graphic.ProcessColor();
            BackColor = backColor;
            ForeColor = foreColor;
            MenuBackColor = pi.Contrast(backColor, 20);
            MenuForeColor = pi.Contrast(foreColor, -20);
            InputBackColor = pi.Contrast(backColor, 40);
            InputForeColor = pi.Contrast(foreColor, -40);
            ButtonBackColor = pi.Contrast(backColor, 15);
            ButtonForeColor = pi.Contrast(foreColor, -15);
            SpecialBackColor = specialBackColor?? pi.Contrast(backColor, 50);
            SpecialForeColor = specialForeColor?? pi.Contrast(foreColor, -50);


            specialBackColor = specialBackColor ?? backColor;
            specialForeColor = specialForeColor ?? foreColor;
            FirstSpecialBackColor = Color.FromArgb(specialBackColor.Value.A, specialBackColor.Value.R/2, specialBackColor.Value.G, specialBackColor.Value.B);
            FirstSpecialForeColor = Color.FromArgb(specialForeColor.Value.A, Math.Min(255, specialForeColor.Value.R*2), specialForeColor.Value.G, specialForeColor.Value.B);
            SecondSpecialBackColor = Color.FromArgb(specialBackColor.Value.A, specialBackColor.Value.R, specialBackColor.Value.G, specialBackColor.Value.B / 2);
            SecondSpecialForeColor = Color.FromArgb(specialForeColor.Value.A, specialForeColor.Value.R, specialForeColor.Value.G, Math.Min(255, specialForeColor.Value.B * 2));
            ThirdSpecialBackColor = Color.FromArgb(specialBackColor.Value.A, specialBackColor.Value.R, specialBackColor.Value.G/2, specialBackColor.Value.B / 2);
            ThirdSpecialForeColor = Color.FromArgb(specialForeColor.Value.A, specialForeColor.Value.R, Math.Min(255, specialForeColor.Value.G*2), Math.Min(255, specialForeColor.Value.B * 2));
          }

    }
}
