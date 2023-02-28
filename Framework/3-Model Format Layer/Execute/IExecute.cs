using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using MiMFa.Model.Structure;
using MiMFa.General;

namespace MiMFa.Framework.ModelFormatLayer.Execute
{
    public interface IExecute<T> where T : StructureBase
    {
        string TableName { get; }
        T DefaultConstructor { get; set; }
        string CREATE_QUERY { get; }
        string SELECT_QUERY { get; }
        string INSERTORREPLACE_QUERY { get; }
        string UPDATE_QUERY { get; }
        string ADDCOLUMN_QUERY { get; }
        string REPLACE_QUERY { get; }
        string DELETE_QUERY { get; }
        string REMOVECOLUMN_QUERY { get; }
        string RENAMECOLUMN_QUERY { get; }
        string TRUNCATE_QUERY { get; }
        string DROP_QUERY { get; }
        bool DefaultIFNotExist { get; set; }
        bool Change { get; set; }
        bool Cash { get; set; }
        string CashCondition { get; set; }
        string[] CashObject { get; set; }
        bool UseCashAllowance(string cashCondition = "", params string[] cashObject);
        T[] CashList { get; set; }
        Dictionary<string, string> ColumnDic { get; set; }

        string SelectCondition { get; set; }

        T DefaultValue(params object[] args);
        T[] DataTableToArray(DataTable dt);

        bool Exist(double value);
        bool Exist(string value);
        bool Exist(string column, string value);
        bool IsEmpty();
        double Count();

        object Create(string attachment);
        void Drop();
        void Truncate();
        object Insert(T obj, string condition);
        object Insert(T[] obj, string condition);
        object SetValue(KeyValuePair<string, string> columnValue, string condition);
        object SetValues(Dictionary<string, string> columnValues, string condition);
        object AddColumn(KeyValuePair<string,string> column, string condition);
        object AddColumns(Dictionary<string, string> columns, string condition);
        DataTable GetDataTable(string condition = "", params string[] column);
        T[] Search(string search);
        T[] Select(string condition, params string[] column);
        object Delete(string condition);
        object RenameColumn(string oldName, string newName, string condition);
        object RemoveColumn(string name, string condition);
        object RemoveColumns(string[] names, string condition);

        T[] Select_All();
        object Delete_All();

        double Get_Max_ID();
        double Get_Min_ID();

        T Select_First();
        object Delete_First();
        T Select_Last();
        object Delete_Last();

        T[] Select_GreaterThan(double id);
        object Delete_GreaterThan(double id);
        T[] Select_LessThan(double id);
        object Delete_LessThan(double id);
        T[] Select_EqualGreaterThan(double id);
        object Delete_EqualGreaterThan(double id);
        T[] Select_EqualLessThan(double id);
        object Delete_EqualLessThan(double id);

        T Select_ByID(long id);
        T Select_ByUID(string uid);
        T Select_ByName(string name);
        T Select_ByDetail(string detail);
        T Select_ByLikedName(string name);
        T[] Select_ByIDs(double[] ids);
        T[] Select_ByUIDs(string[] uids);
        T[] Select_ByNames(string[] names);
        T[] Select_ByDetails(string[] details);
        T[] Select_ByLikedNames(string[] names);
        object Delete_ByID(double id);
        object Delete_ByUID(string uid);
        object Delete_ByName(string name);
        object Delete_ByDetail(string detail);
        object Delete_ByIDs(double[] ids);
        object Delete_ByUIDs(string[] uids);
        object Delete_ByNames(string[] names);
        object Delete_ByDetails(string[] details);
        object Delete_ByLikedNames(string[] names);
    }
}