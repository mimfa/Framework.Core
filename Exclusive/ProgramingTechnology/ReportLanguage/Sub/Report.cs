using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.Service;
using MiMFa;
using System.IO;
using MiMFa.Model.Structure;
using MiMFa.General;

namespace MiMFa.Exclusive.ProgramingTechnology.ReportLanguage
{
    [Serializable]
    public class Report : StructureBase
    {
        public static string Extension = ".mrp";

        public double RSID { get; set; }
        public override string Name { get; set; } = "Default";
        public string Address { get; set; } = "/My Folder/My Reports/";

        public ReportStyle Style { get; set; } = new ReportStyle();
        public object[] ObjectArray { get; set; } = null;
        public Type Type { get; set; } = typeof(Report);
        private string _HTML = "";
        public string HTML
        {
            get
            {
                AccessDate = Default.Date;
                AccessTime = Default.Time;
                return _HTML;
            }
            set { _HTML = value; }
        }
        public SmartDate CreateDate { get; set; } = null;
        public SmartTime CreateTime { get; set; } = null;
        public SmartDate AccessDate { get; set; } = null;
        public SmartTime AccessTime { get; set; } = null;
        public string CreatorName { get; set; } = "";
        public AccessMode Accessablity { get; set; } = AccessMode.User;

        public Report(
            string name = "Default",
            ReportStyle style = null,
            string html = "",
            Object extra = null,
            Type type = null,
            params object[] objectArray)
        {
            Set(
                name,
                style,
                html,
                extra,
                type,
                objectArray);
        }
        public Report(Report report)
        {
            Set( report);
        }
        public void Set(Report report)
        {
            Set(
            report.Name,
            report.Style,
            report.HTML,
            report.Value,
            report.Type,
            report.ObjectArray);
        }
        public void Set(
        string name = "Default",
        ReportStyle style = null,
        string html = "",
        Object extra = null,
        Type type = null,
        params object[] objectArray)
        {
            Name = name;
            if (style != null) Style.Set(style);
            RSID = Style.RSID;
            ObjectArray = objectArray;
            Type = type;
            _HTML = html;
            Value = extra;
            CreateDate = Default.Date;
            CreateTime = Default.Time;
            PathService.CreateAllDirectories(Address);
        }
        public string GetPath()
        {
            return Path.GetFullPath(Address);
        }
    }
}
