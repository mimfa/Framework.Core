using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using MiMFa.Framework.ModelFormatLayer.Execute;
using MiMFa.Exclusive.ProgramingTechnology.ReportLanguage;

namespace MiMFa.Framework.ModelFormatLayer.Execute
{
    [Serializable]
    public class Archive : Execute<Report>
    {
        public Archive(MiMFa.Exclusive.ProgramingTechnology.DataBase.SQLiteDataBase msql) : base(msql)
        {
        }
        public override string TableName => "Reports";
        public override Report DefaultConstructor => new Report();
        public override bool DefaultIFNotExist { get; set; } = false;
        public override string SelectCondition { get; set; } = "ORDER BY ID DESC";
        public override Dictionary<string, string> ColumnDic
        {
            get
            {
                Dictionary<string, string> cdic = new Dictionary<string, string>();
                cdic.Add("ID", MSQL.TYPE.DOUBLE + " NOT NULL");
                cdic.Add("Name", MSQL.TYPE.TEXT);
                cdic.Add("Address", MSQL.TYPE.TEXT);
                cdic.Add("HTML", MSQL.TYPE.TEXT);
                cdic.Add("RSID", MSQL.TYPE.REAL);
                cdic.Add("Style", MSQL.TYPE.BLOB);
                cdic.Add("Type", MSQL.TYPE.BLOB);
                cdic.Add("ObjectArray", MSQL.TYPE.BLOB);
                cdic.Add("CreatorName", MSQL.TYPE.TEXT);
                cdic.Add("CreateDate", MSQL.TYPE.BLOB);
                cdic.Add("CreateTime", MSQL.TYPE.BLOB);
                cdic.Add("Accessablity", MSQL.TYPE.BLOB);
                cdic.Add("AccessDate", MSQL.TYPE.BLOB);
                cdic.Add("AccessTime", MSQL.TYPE.BLOB);
                cdic.Add("Detail", MSQL.TYPE.TEXT);
                cdic.Add("Extra", MSQL.TYPE.OBJECT);
                cdic.Add("UID", MSQL.TYPE.TEXT + " NOT NULL, PRIMARY KEY (ID)");
                return cdic;
            }
        }
    }
}