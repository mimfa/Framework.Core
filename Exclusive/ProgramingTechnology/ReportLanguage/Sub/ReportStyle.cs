using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.Service;
using System.IO;
using MiMFa.Model.Structure;

namespace MiMFa.Exclusive.ProgramingTechnology.ReportLanguage
{
    [Serializable]
    public class ReportStyle : StructureBase
    {
        public static string Extension { get; set; } = ".rps";

        public double RSID { get; set; }
        public override string Name { get; set; } = "Default";
        public string Address { get; set; } = "/My Folder/My ReportStyles/";

        public string MRLCode { get; set; } = "";
        public string Css { get; set; } = "";
        public string Script { get; set; } = "";

        public ReportStyle(
                string name = "Default",
                string mrlCode = "",
                string css = "",
                string script = "" ,
                object extra = null)
        {
            Set(name,
                 mrlCode,
                 css,
                 script,
                 extra);
        }
        public ReportStyle(ReportStyle reportStyle)
        {
            Set(reportStyle);
        }
        public ReportStyle(string reportStylePath)
        {
            ReportStyle reportStyle = new ReportStyle();
            Service.IOService.OpenDeserializeFile(reportStylePath, ref reportStyle);
            reportStyle.Address = reportStylePath;
            Set(reportStyle);
        }
        public void Set(ReportStyle reportStyle)
        {
            Set(reportStyle.Name,
             reportStyle.MRLCode,
             reportStyle.Css,
             reportStyle.Script,
             reportStyle.Value);
        }
        public void Set(
        string name = "Default",
        string mrlCode = "",
        string css = "",
        string script = "",
        object extra = null)
        {
            Name = name;
            MRLCode = mrlCode;
            Css = css;
            Script = script;
            Value = extra;
            PathService.CreateAllDirectories(Address);
        }
        public string GetPath()
        {
            return Path.GetFullPath(Address);
        }

        public static ReportStyle FromAddress(string address)
        {
            return new ReportStyle(address);
        }
    }
}
