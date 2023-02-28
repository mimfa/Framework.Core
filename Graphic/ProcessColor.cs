using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using System.Drawing.Drawing2D;
using MiMFa.Service;

namespace MiMFa.Graphic
{
    public class ProcessColor : ProcessGraphic<Color>
    {
        public override Color FlipHorizental(Color source)
        {
            return Color.FromArgb(
                source.A,
                255 - source.B,
                255 - source.G,
                255 - source.R
            );
        }
        public override Color FlipVertical(Color source)
        {
            return Color.FromArgb(
                source.A,
                255 - source.R,
                255 - source.G,
                255 - source.B
            );
        }
        public override Color Rotate(Color source, float angle)
        {
            angle = angle * 255 / 360;
            return Color.FromArgb(
                source.A,
                ConvertService.TryToInt(Statement.Apply(v=>v<0?v+255:v, (source.R + angle) % 255), 0),
                ConvertService.TryToInt(Statement.Apply(v => v < 0 ? v + 255 : v, (source.G + angle) % 255), 0),
                ConvertService.TryToInt(Statement.Apply(v => v < 0 ? v + 255 : v, (source.B + angle) % 255), 0)
            );
        }
    
        public override Color Change(Color source, Func<Color, Color> colorProcess)
        {
            return colorProcess(source);
        }

        public override Color Combine(params Color[] sources)
        {
            List<int> rColors = new List<int>();
            List<int> gColors = new List<int>();
            List<int> bColors = new List<int>();
            List<int> aColors = new List<int>();
            foreach (Color c in sources)
            {
                rColors.Add(c.R);
                gColors.Add(c.G);
                bColors.Add(c.B);
                aColors.Add(c.A);
            }
            return Color.FromArgb(
                ConvertService.TryToInt(aColors.Average(),0),
                ConvertService.TryToInt(rColors.Average(),0),
                ConvertService.TryToInt(gColors.Average(),0),
                ConvertService.TryToInt(bColors.Average(),0)
            );
        }
    }
}
