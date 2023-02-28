using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Template
{
    public class PaletteBase : IPalette
    {
        public virtual Font Font { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public virtual Font BigFont { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public virtual Font SmallFont { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public virtual Font MenuFont { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public virtual Font LabelFont { get; set; } = null;
        public virtual Font InputFont { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public virtual Font ButtonFont { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        public virtual Font SpecialFont { get; set; } = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        public virtual Color BackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public virtual Color ForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public virtual Color MenuBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public virtual Color LabelBackColor { get; set; } = Color.Empty;
        public virtual Color InputBackColor { get; set; } = Color.FromArgb(253, 253, 253);
        public virtual Color ButtonBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public virtual Color SpecialBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public virtual Color MenuForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public virtual Color LabelForeColor { get; set; } = Color.Empty;
        public virtual Color InputForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public virtual Color ButtonForeColor { get; set; } = Color.FromArgb(25, 25, 25);
        public virtual Color SpecialForeColor { get; set; } = Color.FromArgb(25, 25, 25);

        public virtual Color FirstSpecialBackColor { get; set; } = Color.FromArgb(127, 127, 127);
        public virtual Color SecondSpecialBackColor { get; set; } = Color.FromArgb(127, 127, 127);
        public virtual Color ThirdSpecialBackColor { get; set; } = Color.FromArgb(127, 127, 127);
        public virtual Color FirstSpecialForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public virtual Color SecondSpecialForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public virtual Color ThirdSpecialForeColor { get; set; } = Color.FromArgb(40, 40, 40);

        public virtual Font UpdateFont(string path)
            => UpdateFont(GetOrAddFontFamilly(path));
        public virtual Font UpdateFont(FontFamily fontFamily) 
            => UpdateFont(new Font(fontFamily,Font.Size, Font.Style, Font.Unit));
        public virtual Font UpdateFont(Font font)
        {
            if (font != null)
            {
                Font = new Font(font.FontFamily, font.Size, font.Style, font.Unit);
                BigFont = new Font(font.FontFamily, MathService.Round(font.Size * 1.2F, 2), font.Style, font.Unit);
                SmallFont = new Font(font.FontFamily, MathService.Round(font.Size * 0.8F, 2), font.Style, font.Unit);
                MenuFont = new Font(font.FontFamily, font.Size, font.Style, font.Unit);
                ButtonFont = new Font(font.FontFamily, MathService.Round(font.Size * 0.025F,2), font.Style, font.Unit);
                InputFont = new Font(font.FontFamily, font.Size, font.Style, font.Unit);
                SpecialFont = new Font(font.FontFamily, MathService.Round(font.Size * 1.1F, 2), font.Style, font.Unit);
            }
            return Font;
        }

        #region Font
        public static PrivateFontCollection FontCollection = new PrivateFontCollection();
        public static Dictionary<string, FontFamily> FontFamillies = new Dictionary<string, FontFamily>();
        public FontFamily GetOrAddFontFamilly(string path)
        {
            if (FontFamillies.ContainsKey(path)) return FontFamillies[path];
            var fam = AddFontFamilly(File.ReadAllBytes(path));
            FontFamillies.Add(path, fam);
            return fam;
        }
        public FontFamily AddFontFamilly(byte[] fontBytes)
        {
            var handle = System.Runtime.InteropServices.GCHandle.Alloc(fontBytes, System.Runtime.InteropServices.GCHandleType.Pinned);
            IntPtr pointer = handle.AddrOfPinnedObject();
            try
            {
                FontCollection.AddMemoryFont(pointer, fontBytes.Length);
            }
            finally
            {
                handle.Free();
            }
            return FontCollection.Families.LastOrDefault();
        }
        #endregion
    }
}
