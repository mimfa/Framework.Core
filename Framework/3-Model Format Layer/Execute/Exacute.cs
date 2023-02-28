using MiMFa.General;
using MiMFa.Exclusive.ProgramingTechnology.DataBase;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.Model.Structure;

namespace MiMFa.Framework.ModelFormatLayer.Execute
{
    public class Execute<T> : IExecute<T> where T : StructureBase
    {
        public SQLiteDataBase MSQL;
        public Execute(SQLiteDataBase msql)
        {
            MSQL = msql;
        }
        public virtual string TableName => "";
        public virtual T DefaultConstructor { get; set; } = null;
        public virtual string CREATE_QUERY => string.Join("", "CREATE TABLE IF NOT EXISTS " , TableName ," ");
        public virtual string SELECT_QUERY => string.Join("", "SELECT * FROM " , TableName , " ");
        public virtual string INSERT_QUERY => string.Join("", "INSERT INTO " , TableName , " ");
        public virtual string INSERTORREPLACE_QUERY => string.Join("", "INSERT OR REPLACE INTO " , TableName , " ");
        public virtual string UPDATE_QUERY => string.Join("", "UPDATE " , TableName , " ");
        public string ADDCOLUMN_QUERY => string.Join("", "ALTER TABLE " , TableName , " ADD COLUMN ");
        public string RENAMECOLUMN_QUERY => string.Join("", "ALTER TABLE " , TableName , " ");
        public string REMOVECOLUMN_QUERY => string.Join("", "ALTER TABLE " , TableName , " ");
        public virtual string REPLACE_QUERY => string.Join("", "REPLACE INTO " , TableName, " ");
        public virtual string DELETE_QUERY => string.Join("", "DELETE FROM ", TableName, " ");
        public virtual string TRUNCATE_QUERY => string.Join("", "TRUNCATE TABLE " , TableName , " ");
        public virtual string DROP_QUERY => string.Join("", "DROP TABLE IF EXISTS " , TableName , " ");
        public virtual bool DefaultIFNotExist { get; set; } = false;
        public virtual bool Change { get; set; } = true;
        public virtual bool Cash { get; set; } = true;
        public virtual string CashCondition { get; set; } = "";
        public virtual string[] CashObject { get; set; } = new string[0];
        public virtual T[] CashList { get; set; } = new T[] { };
        public virtual bool UseCashAllowance(string cashCondition = "",params string[] cashObject)
        {
            bool b = !Change && CashCondition == cashCondition && CashObject.Length == cashObject.Length;
            Change = false;
            CashCondition = cashCondition;
            CashObject = cashObject;
            return b;
        }
        public virtual Dictionary<string, string> ColumnDic { get; set; } = new Dictionary<string, string>();

        public virtual string SelectCondition { get; set; } = "ORDER BY ID DESC";

        public virtual bool Exist(double value)
        {
            return MSQL.Exist(TableName, "ID", value.ToString());
        }
        public virtual bool Exist(string condition)
        {
            return MSQL.Exist(TableName, condition);
        }
        public virtual bool Exist(string column, string value)
        {
            return MSQL.Exist(TableName, column, value);
        }
        public virtual bool IsEmpty()
        {
            return MSQL.IsEmpty(TableName);
        }
        public virtual double Count()
        {
            return MSQL.Count(TableName);
        }

        public virtual T DefaultValue(params object[] args)
        {
            return DefaultConstructor;
        }
        public virtual T[] DataTableToArray(DataTable dt)
        {
            T[] lobj = new T[dt.Rows.Count];
            for (int i = 0; i < lobj.Length; i++)
            {
                lobj[i] = DefaultConstructor;
                ConvertService.ToObject(dt, i, ref lobj[i]);
            }
            return lobj;
        }

        public virtual object EXE(string query, ExecuteMode executeType, params SqlParameter[] parameters)
        {
            Change = true;
            return MSQL.Execute(query.Replace("[Table]", TableName), executeType, parameters);
        }

        public virtual void Drop()
        {
            Change = true;
            MSQL.Execute(DROP_QUERY, ExecuteMode.ExecuteNonQuery);
        }
        public virtual void Truncate()
        {
            Change = true;
            MSQL.Execute(TRUNCATE_QUERY, ExecuteMode.ExecuteNonQuery);
        }

        public virtual object Create(string attachment = "")
        {
            if (ColumnDic == null) return null;
            Change = true;
            return MSQL.Execute(string.Join("", CREATE_QUERY , " (" , CollectionService.GetAllItems(ColumnDic," "," , ") , ") " , attachment, ";"), ExecuteMode.ExecuteNonQuery);
        }

        public virtual object Insert(T obj, string condition = "")
        {
            Change = true;
            var q = MSQL.CreateQuery(INSERTORREPLACE_QUERY, ColumnDic, condition, obj);
            return MSQL.Execute(q.Key, ExecuteMode.ExecuteNonQuery, q.Value);
        }
        public virtual object Insert(T[] objs, string condition = "")
        {
            Change = true;
            var q = MSQL.CreateQuery(INSERTORREPLACE_QUERY, ColumnDic, condition, objs);
            return MSQL.Execute(q.Key, ExecuteMode.ExecuteNonQuery,q.Value);
        }
        public object SetValues(Dictionary<string, string> columnValues, string condition)
        {
            Change = true;
            return MSQL.SetValues(TableName,columnValues,condition);
        }
        public object SetValue(KeyValuePair<string, string> columnValue, string condition)
        {
            Change = true;
            return MSQL.SetValue(TableName,columnValue,condition);
        }

        public object AddColumn(KeyValuePair<string, string> column, string condition)
        {
            Change = true;
            var q = string.Join("", ADDCOLUMN_QUERY ," [", column.Key , "] " , column.Value , " " , condition);
            return MSQL.Execute(q, ExecuteMode.ExecuteNonQuery);
        }
        public object AddColumns(Dictionary<string, string> columns, string condition)
        {
            Change = true;
            object o = null;
            foreach (var item in columns)
                o = AddColumn(item, condition);
            return o;
        }
        public object RenameColumn(string oldName, string newName, string condition)
        {
            Change = true;
            throw new NotImplementedException();
        }
        public object RemoveColumn(string name, string condition)
        {
            Change = true;
            throw new NotImplementedException();
        }
        public object RemoveColumns(string[] names, string condition)
        {
            Change = true;
            throw new NotImplementedException();
        }

        public virtual DataTable GetDataTable(string condition = "", params string[] column)
        {
            return MSQL.Select(TableName, condition+ " " + SelectCondition, column);
        }

        public virtual T[] Search(string search)
        {
            Func<string, string> fs = (s) =>  s.Replace("\"", "").Replace("#", "").Replace("$", "");
            if (search.StartsWith("#")) return Select("WHERE ID=" + fs(search));
            if (search.StartsWith("\""))
            {
                search = fs(search);
                return Select(string.Join("", "WHERE Name LIKE '%", search , "%' OR Detail LIKE '%" , search , "%' OR UID LIKE '%" , search , "%' "));
            }
            if (search.StartsWith("$")) return Select("WHERE " + search.Substring(1));
            search = search.Replace(" ", "%");
            return Select(string.Join("", "WHERE Name LIKE '%" , search , "%' OR Detail LIKE '%" ,search , "%' OR UID LIKE '%", search , "%' "));
        }

        public virtual T[] Select(string condition, params string[] column)
        {
            if (Cash)
            {
                if (!UseCashAllowance(condition, column))
                    CashList = DataTableToArray(GetDataTable(condition, column));
                return CashList;
            }
            return DataTableToArray(GetDataTable(condition, column));
        }

        public virtual object Delete(string condition)
        {
            Change = true;
           return MSQL.Execute(string.Join(" ", DELETE_QUERY , condition), ExecuteMode.ExecuteNonQuery);
        }

        public virtual T[] Select_All()
        {
            return Select("");
        }

        public virtual object Delete_All()
        {
            return Delete("");
        }

        public virtual double Get_Max_ID()
        {
            object obj = MSQL.Execute(
                        string.Join("", "SELECT MAX(ID) FROM " , TableName , ";"),
                        ExecuteMode.ExecuteScalar);
            if (obj.GetType() == typeof(DBNull) || obj == null) return 0;
            return System.Convert.ToDouble(obj);
        }
        public virtual double Get_Min_ID()
        {
            object obj = MSQL.Execute(
              string.Join("", "SELECT MIN(ID) FROM " , TableName , ";"),
               ExecuteMode.ExecuteScalar);
            if (obj.GetType() == typeof(DBNull) || obj == null) return 0;
            return System.Convert.ToDouble(obj);
        }

        public virtual T Select_First()
        {
            try { return Select("WHERE ID = " + Get_Min_ID().ToString()).First(); }
            catch { if (DefaultIFNotExist) return DefaultValue(); else return null; }
        }
        public virtual T Select_Last()
        {
            try { return Select("WHERE ID = " + Get_Max_ID().ToString()).First(); }
            catch { if (DefaultIFNotExist) return DefaultValue(); else return null; }
        }

        public virtual object Delete_First()
        {
            return Delete("WHERE ID = " + Get_Min_ID().ToString());
        }
        public virtual object Delete_Last()
        {
            return Delete("WHERE ID = " + Get_Max_ID().ToString());
        }

        public virtual T[] Select_GreaterThan(double id)
        {
            return Select("WHERE ID > " + id);
        }
        public virtual T[] Select_LessThan(double id)
        {
            return Select("WHERE ID < " + id);
        }
        public virtual T[] Select_EqualGreaterThan(double id)
        {
            return Select("WHERE ID >= " + id);
        }
        public virtual T[] Select_EqualLessThan(double id)
        {
            return Select("WHERE ID <= " + id);
        }

        public virtual object Delete_GreaterThan(double id)
        {
            return Delete("WHERE ID > " + id);
        }
        public virtual object Delete_LessThan(double id)
        {
            return Delete("WHERE ID < " + id);
        }
        public virtual object Delete_EqualGreaterThan(double id)
        {
            return Delete("WHERE ID >= " + id);
        }
        public virtual object Delete_EqualLessThan(double id)
        {
            return Delete("WHERE ID <= " + id);
        }

        public virtual T Select_ByID(long id)
        {
            try { return Select("WHERE ID = " + id).First(); }
            catch { if (DefaultIFNotExist) { T def = DefaultValue(); def.ID = id; return def; } else return null; }
        }
        public virtual T Select_ByUID(string uid)
        {
            try { return Select("WHERE UID = \"" + uid+ "\"").First() ; }
            catch { if (DefaultIFNotExist) { T def = DefaultValue(); def.UID = uid; return def; } else return null; }
        }
        public virtual T Select_ByName(string name)
        {
            try { return Select("WHERE Name = \"" + name + "\"").First(); }
            catch { if (DefaultIFNotExist) { T def = DefaultValue(); def.Name = name; return def; } else return null; }
        }
        public virtual T Select_ByDetail(string detail)
        {
            try { return Select("WHERE Detail = \"" + detail + "\"").First(); }
            catch { if (DefaultIFNotExist) { T def = DefaultValue(); def.Details = detail; return def; } else return null; }
        }
        public virtual T Select_ByLikedName(string name)
        {
            try { return Select("WHERE Name LIKE \"%" + name + "%\"").First(); }
            catch { if (DefaultIFNotExist) { T def = DefaultValue(); def.Name = name; return def; } else return null; }
        }

        public virtual T[] Select_ByIDs(double[] ids)
        {
            try {
                return Select(string.Join("", "WHERE ID IN (", string.Join(" , ", ids), ")"));
            }
            catch { return new T[0]; }
        }
        public virtual T[] Select_ByUIDs(string[] uids)
        {
            try
            {
                return Select(string.Join("", "WHERE UID IN ('", string.Join("' , '", uids), "')"));
            }
            catch { return new T[0]; }
        }
        public virtual T[] Select_ByNames(string[] names)
        {
            try
            {
                return Select(string.Join("", "WHERE Name IN ('", string.Join("' , '", names), "')"));
            }
            catch { return new T[0]; }
        }
        public virtual T[] Select_ByDetails(string[] details)
        {
            try
            {
                return Select(string.Join("", "WHERE Detail IN ('", string.Join("' , '", details), "')"));
            }
            catch { return new T[0]; }
        }
        public virtual T[] Select_ByLikedNames(string[] names)
        {
            try
            {
                return Select(string.Join("", "WHERE Name REGEXP '", string.Join("|", names), "'"));
            }
            catch { return new T[0]; }
        }

        public virtual object Delete_ByID(double id)
        {
            return Delete("WHERE ID = " + id);
        }
        public virtual object Delete_ByUID(string uid)
        {
            return Delete("WHERE UID = \"" + uid + "\"");
        }
        public virtual object Delete_ByName(string name)
        {
            return Delete("WHERE Name = \"" + name + "\"");
        }
        public virtual object Delete_ByDetail(string detail)
        {
            return Delete("WHERE Detail = \"" + detail + "\"");
        }


        public virtual object Delete_ByIDs(double[] ids)
        {
            try
            {
                return Delete(string.Join("", "WHERE ID IN (", string.Join(" , ", ids), ")"));
            }
            catch { return new T[0]; }
        }
        public virtual object Delete_ByUIDs(string[] uids)
        {
            try
            {
                return Delete(string.Join("", "WHERE UID IN ('", string.Join("' , '", uids), "')"));
            }
            catch { return new T[0]; }
        }
        public virtual object Delete_ByNames(string[] names)
        {
            try
            {
                return Delete(string.Join("", "WHERE Name IN ('", string.Join("' , '", names), "')"));
            }
            catch { return new T[0]; }
        }
        public virtual object Delete_ByDetails(string[] details)
        {
            try
            {
                return Delete(string.Join("", "WHERE Detail IN ('", string.Join("' , '", details), "')"));
            }
            catch { return new T[0]; }
        }
        public virtual object Delete_ByLikedNames(string[] names)
        {
            try
            {
                return Delete(string.Join("", "WHERE Name REGEXP '", string.Join("|", names), "'"));
            }
            catch { return new T[0]; }
        }

    }
}