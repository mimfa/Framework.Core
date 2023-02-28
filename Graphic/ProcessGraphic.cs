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
using MiMFa.Service;

namespace MiMFa.Graphic
{
    public abstract class ProcessGraphic<T>
    {
        public virtual T Apply(T source, GraphicOptions options)
        {
            if (options.BrightSwitch)
            {
                source = Bright(source, options.BrightVolume);
                source = Contrast(source, options.ContrastVolume);
                source = Light(source, options.LightVolume);
            }
            if (options.ColorSwitch)
            {
                source = Saturate(source, options.RedVolume, options.GreenVolume, options.BlueVolume, options.OpacityVolume);
                source = Grayscale(source, options.GrayscaleVolume);
            }
            if (options.FlipHorizontalSwitch) source = FlipHorizental(source);
            if (options.FlipVerticalSwitch) source = FlipVertical(source);
            if (options.RotateSwitch) source = Rotate(source,options.AngleVolume);
            return source;
        }

        public virtual T FlipHorizental(T source)
        {
            throw new NotImplementedException();
        }
        public virtual T FlipVertical(T source)
        {
            throw new NotImplementedException();
        }
        public virtual T RotateRight(T source)
        {
            return Rotate(source, -90);
        }
        public virtual T RotateLeft(T source)
        {
            return Rotate(source,90);
        }
        public virtual T Rotate(T source, float angle)
        {
            throw new NotImplementedException();
        }

        public virtual Color Grayscale(Color color, int volume)
        {
            if (volume == 0) return color;
            var r = color.R;
            var g = color.G;
            var b = color.B;
            var mean = (r + g + b) / 3F;
            var n = volume / 255F;
            var o = 1 - n;
            return Color.FromArgb(color.A, Limition(Convert.ToInt32(r * o + mean * n)), Limition(Convert.ToInt32(g * o + mean * n)), Limition(Convert.ToInt32(b * o + mean * n)));
        }
        public virtual T Grayscale(T source, int volume = 0)
        {
            if (volume == 0) return source;
            return Colorate(source, c => Grayscale(c,volume));
        }
        public virtual T Invert(T source, int volume = 0)
        {
            if (volume == 0) return source;
            var perc = volume / 255;
            return Colorate(source, c => 255 - c.R * perc, c => 255 - c.G * perc, c => 255 - c.B * perc, c => c.A);
        }
        public virtual T Contrast(T source, int volume = 0)
        {
            if (volume == 0) return source;
            return Colorate(source, c => {
                var v = (c.R + c.G + c.B) / 3F < 127 ? -volume/255F :volume/255F;
                return Color.FromArgb(c.A, Limition(c.R + v * c.R), Limition(c.G+v * c.G), Limition(c.B+v * c.B));
            });
        }
        public virtual T Bright(T source, int volume = 0)
        {
            if (volume == 0) return source;
            var v = volume / 255F;
            return Colorate(source, c => Color.FromArgb(c.A, Limition(c.R + v * c.R), Limition(c.G + v * c.G), Limition(c.B + v * c.B)));
        }
        public virtual T Light(T source, int volume = 0)
        {
            if (volume == 0) return source;
            return Saturate(source, volume, volume, volume, 0);
        }
        public virtual T Saturate(T source, int addR = 0, int addG = 0, int addB = 0, int addAlpha = 0)
        {
            return Colorate(source, c => c.R + addR, c => c.G + addG, c => c.B + addB, c => c.A + addAlpha);
        }
        public virtual T Colorate(T source, Func<Color, int> redProcess, Func<Color, int> greenProcess, Func<Color, int> blueProcess, Func<Color, int> alphaProcess)
        {
            return Colorate(source,c=> Color.FromArgb(Limition(alphaProcess(c)), Limition(redProcess(c)), Limition(greenProcess(c)), Limition(blueProcess(c))));
        }
        public virtual T Colorate(T source, Func<Color, Color> colorProcess)
        {
            return Change(source, c => c.A>0? colorProcess(c):c);
        }
        public virtual T Change(T source, Func<Color, int> redProcess, Func<Color, int> greenProcess, Func<Color, int> blueProcess, Func<Color, int> alphaProcess)
        {
            return Change(source, c => Color.FromArgb(Limition(alphaProcess(c)), Limition(redProcess(c)), Limition(greenProcess(c)), Limition(blueProcess(c))));
        }
        public virtual T Change(T source, Func<Color, Color> colorProcess)
        {
            throw new NotImplementedException();
        }

        public virtual T Combine(params T[] sources)
        {
            throw new NotImplementedException();
        }

        public virtual int Limition(int value, int min = 0, int max = 255) => Math.Max(min, Math.Min(max, value));
        public virtual int Limition(double value, int min = 0, int max = 255) => Limition(Convert.ToInt32(value),min, max);
    }
}
