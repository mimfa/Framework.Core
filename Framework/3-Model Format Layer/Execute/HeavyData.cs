using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiMFa.General;


namespace MiMFa.Framework.ModelFormatLayer.Execute
{
    public class HeavyData : Execute<Model.HeavyData>
    {
        public HeavyData(Exclusive.ProgramingTechnology.DataBase.SQLiteDataBase msql):base(msql) { }
        public override string TableName => "HeavyData";
        public override  Model.HeavyData DefaultConstructor => new Model.HeavyData();
        public override bool DefaultIFNotExist { get; set; } = true;
        public override Dictionary<string, string> ColumnDic
        {
            get
            {
                Dictionary<string, string> cdic = new Dictionary<string, string>();
                cdic.Add("ID", MSQL.TYPE.DOUBLE);
                cdic.Add("Name", MSQL.TYPE.TEXT);
                cdic.Add("FromTable", MSQL.TYPE.TEXT + " NOT NULL");
                cdic.Add("FromColumn", MSQL.TYPE.TEXT + " NOT NULL");
                cdic.Add("FromRecord", MSQL.TYPE.TEXT + " NOT NULL");
                cdic.Add("Value", MSQL.TYPE.OBJECT);
                cdic.Add("ValueType", MSQL.TYPE.TEXT);
                cdic.Add("Detail", MSQL.TYPE.TEXT);
                cdic.Add("Extra", MSQL.TYPE.OBJECT + " , PRIMARY KEY (FromTable, FromColumn, FromRecord)");
                return cdic;
            }
        }

        public virtual object TableReady(string tableName,string recordColumnName ,string conditions = "")
        {
            return MSQL.Execute(@"
CREATE TRIGGER IF NOT EXISTS Delete_HeavyData 
AFTER DELETE ON " + tableName + @" 
BEGIN 
DELETE FROM " + TableName + " WHERE FromTable = \""+ tableName + "\" AND FromRecord = CAST(OLD."+ recordColumnName+ " AS TEXT) " + conditions +
";END;", ExecuteMode.ExecuteNonQuery);
        }
        public override object Create(string attachment = "")
        {
            base.Create(attachment);
            return MSQL.Execute(@"
CREATE TRIGGER IF NOT EXISTS Update_ID_INSERT 
AFTER INSERT ON " + TableName + @"  
BEGIN 
UPDATE " + TableName + @" SET ID = (SELECT COALESCE(MAX(ID),0) + 1 FROM " + TableName + @");
END;", ExecuteMode.ExecuteNonQuery);
        }
        public object Insert_ByCoordinates(string fromTable, string fromColumn, string fromRecord, object value, string conditions = "")
        {
            Model.HeavyData obj = new Model.HeavyData();
            obj.Name = nameof(value);
            obj.FromTable = fromTable;
            obj.FromColumn = fromColumn;
            obj.FromRecord = fromRecord;
            obj.Value = value;
            obj.ValueType = (value != null)?value.GetType().Name:"Null";
            return Insert(obj, conditions);
        }
        public Model.HeavyData Select_ByCoordinates(string fromTable, string fromColumn, string fromRecord)
        {
            try { return Select(" WHERE FromTable = \"" + fromTable + "\" AND FromColumn = \"" + fromColumn + "\" AND FromRecord = \"" + fromRecord +"\"").First(); }
            catch
            {
                if (DefaultIFNotExist)
                {
                    var obj = DefaultValue();
                    obj.FromTable = fromTable;
                    obj.FromColumn = fromColumn;
                    obj.FromRecord = fromRecord;
                    return obj;
                }
                else return null;
            }
        }
        public object Delete_ByCoordinates(string fromTable, string fromColumn, string fromRecord)
        {
            return Delete("WHERE FromTable = \"" + fromTable + "\" AND FromColumn = \"" + fromColumn + "\" AND FromRecord = \"" + fromRecord + "\"");
        }
    }
}