using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using MiMFa.Service;
using MiMFa.General;
using MiMFa.Model;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace MiMFa.Exclusive.ProgramingTechnology.DataBase
{
    public interface IDataBaseTypes
    {
        string INT { get; set; }
        string INTEGER { get; set; }
        string TINYINT { get; set; }
        string SMALLINT { get; set; }
        string MEDIUMINT { get; set; }
        string BIGINT { get; set; }
        string UNSIGNEDBIGINT { get; set; }
        string INT2 { get; set; }
        string INT8 { get; set; }

        string CHARACTER { get; set; }
        string VARCHAR { get; set; }
        string VARYINGCHARACTER { get; set; }
        string NCHAR { get; set; }
        string NTEXT { get; set; }
        string NVARCHAR { get; set; }
        string TEXT { get; set; }
        string CLOB { get; set; }

        string BLOB { get; set; }
        string OBJECT { get; set; }
        string IMAGE { get; set; }

        string REAL { get; set; }
        string DOUBLE { get; set; }
        string DECIMAL { get; set; }
        string DOUBLE_PRECISION { get; set; }
        string FLOAT { get; set; }

        string NUMERIC { get; set; }
        string BOOL { get; set; }
        string BOOLEAN { get; set; }
        string DATE { get; set; }
        string TIME { get; set; }
        string DATETIME { get; set; }
    }
    public interface IDataBase : IProgramingTechnology
    {
        IDataBaseTypes TYPE { get; set; }

        bool ShowException { get; set; }
        IDbConnection Connection { get; set; }
        IDbCommand Command { get; set; }
        IDbDataAdapter Adapter { get; set; }
        DataTable Result { get; set; }

        void Start(string address, string password = "", decimal version = 3, int timeout = 3000, bool uTF8Encoding = true, string tableName = null, params string[] columnsname);

        void Open();
        void Close();

        object Execute(string query, ExecuteMode executeType, params DbParameter[] parameters);
        object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string[] names, params object[] parameters);
        object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, params DbParameter[] parameters);
        object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string appendQuery, params DbParameter[] parameters);
        object Execute(string query, ExecuteMode executeType, DbParameterCollection parameters);
        object Execute(string query, ExecuteMode executeType);
        object Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string appendQuery, T thisClass);
        object Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, T thisClass);
        object Execute<T>(string query, ExecuteMode executeType, T thisClass);
        List<object> Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string appendQuery, params T[] classes);
        List<object> Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, params T[] classes);
        List<object> Execute<T>(string query, ExecuteMode executeType, params T[] classes);
        object Create(string tableName, Dictionary<string, string> columnDic, string attachment = "");
        object Create(string tableName, string columns, string attachment = "");
        DataTable Select(string tableName, string condition = "");
        DataTable Select(string tableName, double id);
        DataTable SelectFirst(string tableName);
        DataTable SelectLast(string tableName);
        object Insert(string tableName, string[] columnNames, object[] objects, string condition);
        List<object> Insert(string tableName, Dictionary<string, string> columnDic, object[] obj, string condition = ""); object Insert(string tableName, Dictionary<string, string> columnDic, object obj, string condition = "");
        object Insert(string tableName, string columns, string values, string condition = "", DbParameter[] parameters = null);
        object Insert(string tableName, string condition = "", DbParameter[] parameters = null);
        object GetColumn(string tableName, string columnName, string condition = null);
        void CreateHeavyData(string tableName, string valueType = "BLOB");
        object GetFromHeavyData(string tableName, string FromTable, string inColumn, string FromRecord);
        object SetToHeavyData(string tableName, string FromTable, string inColumn, string FromRecord, object value, string condition = "");
        void CreateDic(string tableName, string valueType = "TEXT");
        Dictionary<string, T> GetDic<T>(string tableName, string condition = "");
        object[] GetKeys(string tableName, string value, string condition = "", bool blobValue = false);
        object GetValue(string tableName, string key, string condition, bool blobValue = false);
        object GetValue(string tableName, string key, bool blobValue = false);
        object SetValue(string tableName, string key, object value, bool blobValue = false);
        object SetValue(string tableName, string key, object value, string condition, bool blobValue = false);
    }
}
