using MiMFa.Exclusive.ProgramingTechnology.CommandLanguage;
using MiMFa.General;
using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.View
{
    public class ToHTML : ToStringBase
    {
        public Size Size { get; set; } = new Size(0,0);
        public override bool Translate { get; set; } = false;

        public override string StartSign { get; set; } = "";
        public override string MiddleSign { get; set; } = "->";
        public override string EndSign { get; set; } = "";
        public override string SplitSign { get; set; } = " ";
        public override string BreakSign { get; set; } = "<br>";
        public override string TabSign { get; set; } = "\t ";
        public override string BreakLineSign { get; set; } = "<hr>";
        public override string Table_ { get; set; } = "<table>";
        public override string TableRow_ { get; set; } = "<tr>";
        public override string TableCell_ { get; set; } = "<td>";
        public override string _TableCell { get; set; } = "</td>";
        public override string _TableRow { get; set; } = "</tr>";
        public override string _Table { get; set; } = "</table>";
        public virtual string Hilight_ { get; set; } = "<span style='color: white; background-color: blue;'>";
        public virtual string _Hilight { get; set; } = "</span>";
        public override Func<string, string> Highlight { get; set; } = (s) => "<span style='color: red; background-color: yellow;'>" + s + "</span>"; 
        
        public override String Done(string arg)
        {
            return ConvertService.ToHTML(arg);
        }
        public override String Done(EventPack arg)
        {
            string str = "";
            str += "<input type='button' value='" + Done("Open") + "' " + arg.Before;
            str += " "+ arg.Name + "='";
            str +=  arg.Target + "' ";
            str += arg.After + "' />";
            return str;
        }
        public override String Done(Bitmap arg)
        {
            if (arg == null) return "";
            //string address = TempDirectory + DateTime.Now.Ticks + ".img";
            //arg.Save(address);
            string size = "";
            float h = Size.Height * 1F / arg.Height;
            float w = Size.Width * 1F / arg.Width;
            if (h <= w) size += "height:100%;";
            else size += "width:100%;";
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            arg.Save(stream,System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageBytes = stream.ToArray();
            stream.Close();
            return "<img src='data:image/png;base64," + Convert.ToBase64String(imageBytes) + "' class='AutoIMG' style='max-width:100%;max-height:100%;"+ size+"'/>";
            //return "<img src='" + address + "' class='AutoIMG' style='max-width:100%;max-height:100%;"+ size+"'/>";
        }
        public override String Done(Uri arg)
        {
            return "<a href='" + arg.OriginalString + "'>" + Done("Open") + "</a>";
        }
        public override String Done(byte[] arg)
        {
            if (arg == null) return "";
            string ext = "data";
            try { ext = InfoService.GetMimeObject(arg).Split('/').Last().Trim().Split(' ').First(); } catch { }
            string address = TemporaryDirectory + DateTime.Now.Ticks + "." +( (ext == "unknown")?"mp4": ext);
            System.IO.File.WriteAllBytes(address, arg);
            return "<button src='" + address + "' class='AutoBTN' >" + Done("Download") + "</button>";
            //return @"<a href='" + address+"' class='AutoBTN' >" + Done("Download") + "</a>";
        }

    }
}
