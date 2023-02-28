using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using MiMFa.Model.Structure;
using MiMFa.Framework.ModelFormatLayer.Execute;
using MiMFa.Model;
using MiMFa.Exclusive.ProgramingTechnology.ReportLanguage;

namespace MiMFa.Framework.ModelFormatLayer.Execute
{
    public class ReportStyles : Execute<ReportStyle>
    {
        public ReportStyles(MiMFa.Exclusive.ProgramingTechnology.DataBase.SQLiteDataBase msql) : base(msql)
        {
        }
        public override string TableName => "ReportStyles";
        public override ReportStyle DefaultConstructor => new ReportStyle();
        public override bool DefaultIFNotExist { get; set; } = false;
        public override Dictionary<string, string> ColumnDic
        {
            get
            {
                Dictionary<string, string> cdic = new Dictionary<string, string>();
                cdic.Add("RSID", MSQL.TYPE.DOUBLE + " NOT NULL");
                cdic.Add("Style", MSQL.TYPE.BLOB);
                cdic.Add("UID", MSQL.TYPE.TEXT + " NOT NULL, PRIMARY KEY (ID)");
                return cdic;
            }
        }
    }
}