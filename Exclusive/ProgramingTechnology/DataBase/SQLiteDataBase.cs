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
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Data.Common;
using System.Drawing;
using MiMFa.Exclusive.ProgramingTechnology.Tools;
using System.Collections;

namespace MiMFa.Exclusive.ProgramingTechnology.DataBase
{
    [Serializable]
    public class SQLiteDataBase : IProgramingTechnology
    {
        public string Name => "MiMFa SQLite";
        public string Description => "A suitable tool for connecting and use from SQLite database";
        public string TempDirectory
        {
            get { return _TempDirectory; }
            set { PathService.CreateAllDirectories(_TempDirectory = value); }
        }
        public Version Version => new Version(1, 0, 0, 0);

        #region Properties
        public SQLiteDataBaseTypes TYPE { get; set; } = new SQLiteDataBaseTypes();
        public  bool ShowException { get; set; } = false;
        public  SQLiteConnection Connection { get; set; }
        public  SQLiteCommand Command { get; set; }
        public  SQLiteDataAdapter Adapter { get; set; }
        public  DataTable Result { get; set; }
        public string DBPath { get; set; } = "";

        public int WaitForSignal { get; set; } = 10;
        public int SignalDelay { get; set; } = 500;

        public bool IsOpen { get; set; } = false;
        public bool IsChanged { get; set; } = false;

        public event ActionEventHandler ExecuteStart = (s, a) => { };
        public event ActionEventHandler Executing = (s, a) => { };
        public event ActionResultEventHandler ExecuteFinish = (s, a, o) => {  };
        public event ActionErrorEventHandler ExecuteError = (s, a, o) => {  };
        #endregion

        #region Main
        public SQLiteDataBase(string address = null,string password = "", decimal version = 3, int timeout = 3000, bool uTF8Encoding = true, string tableName = null, params string[] columnsname)
        {
            ExecuteStart += (s, e) => IsOpen = true;
            ExecuteFinish += (s, e,o) => IsOpen = false;
            ExecuteError += (s, e,o) => IsOpen = false;
            Start(address,password,version, timeout, uTF8Encoding, tableName, columnsname);
        }

        public  void Start(string address,string password = "",decimal version = 3,int timeout = 3000,bool uTF8Encoding = true, string tableName = null, params string[] columnsname)
        {
            if (string.IsNullOrWhiteSpace(address)) return;
            try
            {
                DBPath = address;
                Command = new SQLiteCommand();
                address = address.Replace("|DataDirectory|\\", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
                Connection = new SQLiteConnection(string.Join("", @"Data Source = " , address , "; Version = " , version , "; UTF8Encoding = " , uTF8Encoding , "; Timeout = " , timeout , ";" , ((string.IsNullOrEmpty(password)) ? "" : " Password=" + password + ";")));
                if (tableName != null)
                {
                    string CommandText = string.Join("", "CREATE TABLE IF NOT EXISTS " , tableName, " (",string.Join(" , ", columnsname), ");");
                    Execute(CommandText, ExecuteMode.ExecuteNonQuery);
                }
                if (!File.Exists(address)) SQLiteConnection.CreateFile(address);
            }
            catch { }
        }

        public void ChangePassword(string pass = "")
        {
            try
            {
                Connection.Open();
                Connection.ChangePassword(pass);
                Connection.Close();
            }
            catch(Exception ex) { if (ShowException) throw new System.Exception(ex.Message); }
        }

        public void Open() => Connection.Open();
        public void Close()
        {
            Command = null;
            Adapter = null;
            Connection.Close();
            Connection.Dispose();
            Connection = null;
            SQLiteConnection.ClearAllPools();
        }
        #endregion

        #region Execute
        public object Execute(string query, ExecuteMode executeType, params SQLiteParameter[] parameters)
        {
            if (IsOpen)
            {
                Statement.LimitedLoop(() => IsOpen, WaitForSignal, () => { Thread.Sleep(SignalDelay); });
                //return Execute(query, executeType, parameters);
            }
            else
            {
                try
                {
                    ExecuteStart(this, query);
                    Connection.Close();
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    SQLiteTransaction transaction = Connection.BeginTransaction();
                    Command.Transaction = transaction;
                    Command.CommandText = query;
                    if (parameters != null) Command.Parameters.AddRange(parameters);
                    Executing(this, query);
                    try
                    {
                        try
                        {
                            switch (executeType)
                            {
                                case ExecuteMode.ExecuteScalar:
                                    object s = Command.ExecuteScalar();
                                    transaction.Commit();
                                    ExecuteFinish(this, query, s);
                                    return s;
                                case ExecuteMode.ExecuteScalarAsync:
                                    Task<object> ts = Command.ExecuteScalarAsync();
                                    transaction.Commit();
                                    ExecuteFinish(this, query, ts);
                                    return ts;
                                case ExecuteMode.ExecuteNonQuery:
                                    IsChanged = true;
                                    int r = Command.ExecuteNonQuery();
                                    transaction.Commit();
                                    ExecuteFinish(this, query, r);
                                    return r;
                                case ExecuteMode.ExecuteNonQueryAsync:
                                    IsChanged = true;
                                    Task<int> t = Command.ExecuteNonQueryAsync();
                                    transaction.Commit();
                                    ExecuteFinish(this, query, t);
                                    return t;
                                default:
                                    Result = new DataTable();
                                    if (parameters != null)
                                        for (int i = 0; i < Command.Parameters.Count; i++)
                                            Result.Columns.Add(Command.Parameters[i].ParameterName.Replace("@", ""), typeof(object));
                                    Adapter = new SQLiteDataAdapter(Command);
                                    Adapter.Fill(Result);
                                    Adapter.Update(Result);
                                    //DataTable dt = new DataTable();
                                    //for (int i = 0; i < Result.Columns.Count; i++)
                                    //    dt.Columns.Add(Result.Columns[i].ColumnName, typeof(object));
                                    //for (int i = 0; i < Result.Rows.Count; i++)
                                    //    dt.Rows.Add();
                                    for (int i = 0; i < Result.Rows.Count; i++)
                                        for (int j = 0; j < Result.Rows[i].ItemArray.Length; j++)
                                            Result.Rows[i][j] = TYPE.Get(Result.Rows[i][j]);
                                    //Result = dt;
                                    break;
                            }
                        }
                        catch 
                        {
                            Result = new DataTable();
                            Adapter = new SQLiteDataAdapter(Command);
                            switch (executeType)
                            {
                                case ExecuteMode.ExecuteScalar:
                                case ExecuteMode.ExecuteScalarAsync:
                                case ExecuteMode.ExecuteNonQuery:
                                case ExecuteMode.ExecuteNonQueryAsync:
                                    object r = Adapter.Update(Result);
                                    transaction.Commit();
                                    ExecuteFinish(this, query, r);
                                    return r;
                                case ExecuteMode.Null:
                                case ExecuteMode.ExecuteReader:
                                case ExecuteMode.ExecuteReaderAsync:
                                    Adapter.Fill(Result);
                                    DataTable dt = new DataTable();
                                    for (int i = 0; i < Result.Columns.Count; i++)
                                        dt.Columns.Add(Result.Columns[i].ColumnName, typeof(object));
                                    for (int i = 0; i < Result.Rows.Count; i++)
                                        dt.Rows.Add();
                                    for (int i = 0; i < Result.Rows.Count; i++)
                                        for (int j = 0; j < Result.Rows[i].ItemArray.Length; j++)
                                            dt.Rows[i][j] = TYPE.Get(Result.Rows[i][j]);
                                    Result = dt;
                                    break;
                            }
                        }
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        ExecuteError(this, query, ex);
                        if (ShowException) throw new System.Exception(ex.Message);
                    }
                }
                catch (System.Exception ex)
                {
                    ExecuteError(this, query, ex);
                    if (ShowException) throw new System.Exception(ex.Message);
                }
                finally { try { Connection.Close(); } catch (Exception ex) { ExecuteError(this, query, ex); } }
                ExecuteFinish(this, query, Result);
            }
                return Result;
        }
        public  object Execute(string query, ExecuteMode executeType)
        {
            SQLiteParameter[] sqlp = null;
            return Execute(query, executeType, sqlp);
        }
        public  object Execute(string query, ExecuteMode executeType, SQLiteParameterCollection parameters)
        {
            SQLiteParameter[] par = new SQLiteParameter[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
                par[i] = (SQLiteParameter)parameters[i];
            return Execute(query,executeType, par);
        }
        public object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName,string[] names,params object[] objects)
        {
            SQLiteParameter[] newParameters = new SQLiteParameter[objects.Length];
            int length = Math.Min(names.Length, objects.Length);
            for (int i = 0; i < length; i++)
                newParameters[i] = new SQLiteParameter("@" + names[i], objects[i]);
            return Execute(query,executeType, createParameterName, createColumnName, newParameters);
        }
        public  object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, params SQLiteParameter[] parameters)
        {
            return Execute(query, executeType, createParameterName, createColumnName, "", parameters);
        }
        public  object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string appendQuery, params SQLiteParameter[] parameters)
        {
            return Execute(CreateQuery(query,createParameterName,  createColumnName,  appendQuery,parameters), executeType, parameters);
        }
        public object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName,Dictionary<string,object> namesANDobjects)
        {
            return Execute(query,executeType, createParameterName, createColumnName, namesANDobjects.Keys.ToArray(), namesANDobjects.Values.ToArray());
        }
        public object Execute(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, object[] objarr)
        {
            Dictionary<string, object> columnDic = new Dictionary<string, object>();
            for (int i = 0; i < objarr.Length; i++)
                columnDic.Add("Col_"+i, objarr[i]);
            return Execute(query, executeType,createParameterName, createColumnName, columnDic);
        }
        public  object Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, T thisClass)
        {
            return Execute(query, executeType, createParameterName, createColumnName, "", thisClass);
        }
        public  object Execute<T>(string query, ExecuteMode executeType, T thisClass)
        {
            return Execute(query, executeType, false, false, "", thisClass);
        }
        public  object Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string appendQuery, T thisClass)
        {
            var q = CreateQuery(query, createParameterName,  createColumnName,  appendQuery, thisClass);
            return Execute(q.Key, executeType, createParameterName, createColumnName, appendQuery, q.Value);
        }
        public  object Execute<T>(string query, ExecuteMode executeType, Dictionary<string, string> columnDic, bool createParameterName, bool createColumnName, string appendQuery, params T[] classes)
        {
            var q = CreateQuery(query, columnDic, createParameterName, createColumnName, appendQuery, classes);
            return Execute(q.Key, executeType, q.Value);
        }
        public object Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, string appendQuery, params T[] classes)
        {
            Type type = typeof(T);
            FieldInfo[] fi = type.GetFields();
            PropertyInfo[] pi = type.GetProperties();
            Dictionary<string, string> columnDic = new Dictionary<string, string>();
            foreach (T thisClass in classes)
            {
                foreach (var item in fi) try
                    {
                        columnDic.Add(item.Name, item.FieldType.Name);
                    }
                    catch { }
                foreach (var item in pi) try
                    {
                        columnDic.Add(item.Name, item.PropertyType.Name);
                    }
                    catch { }
            }
            return Execute(query, executeType,columnDic, createParameterName, createColumnName, "", classes);
        }
        public object Execute<T>(string query, ExecuteMode executeType, bool createParameterName, bool createColumnName, params T[] classes)
        {
            return Execute(query, executeType, createParameterName, createColumnName, "", classes);
        }
        public object Execute<T>(string query, ExecuteMode executeType, params T[] classes)
        {
            return Execute(query, executeType, false, false, "", classes);
        }
        public object Execute<T>(string query, ExecuteMode executeType, DataTable table)
        {
            if (IsOpen)
            {
                Statement.LimitedLoop(() => IsOpen, WaitForSignal, () => { Thread.Sleep(SignalDelay); });
                //return Execute(query, executeType, parameters);
            }
            else
            {
                try
                {
                    ExecuteStart(this, query);
                    Connection.Close();
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    SQLiteTransaction transaction = Connection.BeginTransaction();
                    Command.Transaction = transaction;
                    Command.CommandText = query;
                    Executing(this, query);
                    try
                    {
                        switch (executeType)
                        {
                            case ExecuteMode.ExecuteScalar:
                            case ExecuteMode.ExecuteScalarAsync:
                                Result = new DataTable();
                                Adapter = new SQLiteDataAdapter(Command);
                                Adapter.Fill(Result);
                                table.Clear();
                                for (int i = 0; i < Result.Columns.Count; i++)
                                    table.Columns.Add(Result.Columns[i].ColumnName, typeof(object));
                                for (int i = 0; i < Result.Rows.Count; i++)
                                    table.Rows.Add();
                                for (int i = 0; i < Result.Rows.Count; i++)
                                    for (int j = 0; j < Result.Rows[i].ItemArray.Length; j++)
                                        table.Rows[i][j] = TYPE.Get(Result.Rows[i][j]);
                                transaction.Commit();
                                ExecuteFinish(this, query, table);
                                return Result = table;
                            case ExecuteMode.ExecuteNonQuery:
                            case ExecuteMode.ExecuteNonQueryAsync:
                                Adapter = new SQLiteDataAdapter(Command);
                                int r = Adapter.Update(table);
                                transaction.Commit();
                                ExecuteFinish(this, query, r);
                                return r;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        ExecuteError(this, query, ex);
                        if (ShowException) throw new System.Exception(ex.Message);
                    }
                    finally
                    {
                        try { Connection.Close(); }
                        catch (Exception ex) { ExecuteError(this, query, ex); }
                    }
                }
                catch (System.Exception ex)
                {
                    ExecuteError(this, query, ex);
                    if (ShowException) throw new System.Exception(ex.Message);
                }
            }
            ExecuteFinish(this, query, Result);
            return Result;
        }

        char SeprateChar = '*';
        public object MergeQuery(ExecuteMode executeType, params KeyValuePair<string, SQLiteParameter[]>[] queries)
        {
            string query = CollectionService.GetAllItems((from v in queries select v.Key).ToArray(), "; ");
            SQLiteParameter[] par = CollectionService.Concat((from v in queries select v.Value).ToArray());
            return Execute(query, executeType, par);
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, T thisClass)
        {
            Type type = typeof(T);
            FieldInfo[] fi = type.GetFields();
            PropertyInfo[] pi = type.GetProperties();
            List<SQLiteParameter> lsp = new List<SQLiteParameter>();
            string st = SeprateChar + UniqueService.CreateNewString(20);
            foreach (var item in fi) try
                {
                    var obj = item.GetValue(thisClass);
                    if (obj != null)
                        if (obj is System.Drawing.Image)
                            obj = ConvertService.ToByteArray((System.Drawing.Image)obj);
                        else if (!InfoService.IsNumber(obj)
                            && !InfoService.IsText(obj))
                            obj = IOService.Serialize(obj);
                    lsp.Add(new SQLiteParameter(string.Join("", "@", item.Name , st), obj));
                }
                catch { }
            foreach (var item in pi) try
                {
                    var obj = item.GetValue(thisClass);
                    if (obj != null)
                        if (obj is System.Drawing.Image)
                            obj = ConvertService.ToByteArray((System.Drawing.Image)obj);
                        else if (!InfoService.IsNumber(obj)
                            && !InfoService.IsText(obj))
                            obj = IOService.Serialize(obj);
                    lsp.Add(new SQLiteParameter(string.Join("", "@" , item.Name , st), obj));
                }
                catch { }
            return new KeyValuePair<string, SQLiteParameter[]>(CreateQuery(query, true, true, "", lsp.ToArray()), lsp.ToArray());
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, string appendQuery, T thisClass)
        {
            return CreateQuery(query, true, true, appendQuery, thisClass);
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, bool createParameterName, bool createColumnName, string appendQuery, T thisClass)
        {
            Type type = typeof(T);
            FieldInfo[] fi = type.GetFields();
            PropertyInfo[] pi = type.GetProperties();
            List<SQLiteParameter> lsp = new List<SQLiteParameter>();
            string st = SeprateChar + UniqueService.CreateNewString(20);
            foreach (var item in fi) try
                {
                    var obj = item.GetValue(thisClass);
                    if (obj != null)
                        if (obj is System.Drawing.Image)
                            obj = ConvertService.ToByteArray((System.Drawing.Image)obj);
                        else if (!InfoService.IsNumber(obj)
                            && !InfoService.IsText(obj))
                            obj = IOService.Serialize(obj);
                    lsp.Add(new SQLiteParameter(string.Join("", "@" , item.Name , st), obj));
                }
                catch { }
            foreach (var item in pi) try
                {
                    var obj = item.GetValue(thisClass);
                    if (obj != null)
                        if (obj is System.Drawing.Image)
                            obj = ConvertService.ToByteArray((System.Drawing.Image)obj);
                        else if (!InfoService.IsNumber(obj)
                            && !InfoService.IsText(obj))
                            obj = IOService.Serialize(obj);
                    lsp.Add(new SQLiteParameter(string.Join("", "@" , item.Name , st), obj));
                }
                catch { }
            return new KeyValuePair<string, SQLiteParameter[]>(CreateQuery(query, createParameterName, createColumnName, appendQuery, lsp.ToArray()), lsp.ToArray());
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, Dictionary<string, string> columnDic, string appendQuery, params T[] classes)
        {
            return CreateQuery(query, columnDic,true,true, appendQuery,"",classes);
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, Dictionary<string, string> columnDic, string appendQuery, string suffix, params T[] classes)
        {
            return CreateQuery(query, columnDic,true,true, appendQuery,suffix,classes);
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, Dictionary<string, string> columnDic, bool createParameterName, bool createColumnName, string appendQuery, params T[] classes)
        {
            return CreateQuery(query, columnDic,true,true, appendQuery, "", classes);
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, Dictionary<string, string> columnDic, bool createParameterName, bool createColumnName, string appendQuery,string suffix, params T[] classes)
        {
            Type type = typeof(T);
            string columns = CollectionService.GetAllKeysItem(columnDic, " , ");
            List<string> queries =new List<string>();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            int len = columnDic.Count;
            bool brk = false;
            for (int j = 0; j < classes.Length; j++)
            {
                 brk = false;
                string st = string.Join("", SeprateChar , UniqueService.CreateNewDouble() , suffix);
                List<string> values = new List<string>();
                foreach (var item in columnDic)
                {
                    string val = string.Join("", "@" ,item.Key , st , j);
                    values.Add(val);
                    object o = null;
                    PropertyInfo[] pi = InfoService.GetProperties(item.Key, type, true);
                    if (pi.Length > 0)
                    {
                        o = pi.First().GetValue(classes[j]);
                    }
                    else
                    {
                        FieldInfo[] fi = InfoService.GetFields(item.Key, type, true);
                        if (fi.Length > 0) o = fi.First().GetValue(classes[j]);
                        else brk = true;
                    }
                    if (!brk) parameters.Add(new SQLiteParameter(val, TYPE.Set(o, item.Value)));
                }
                if (!brk) queries.Add( string.Join("", query, (createColumnName ? " (" + columns + ") " : " ") , (createParameterName ? " VALUES(" + string.Join(" , ", values) + ") " : " "), appendQuery , "; "));
            }
            return new KeyValuePair<string, SQLiteParameter[]>(string.Join("",queries), parameters.ToArray());
        }
        public KeyValuePair<string, SQLiteParameter[]> CreateQuery<T>(string query, DataTable dt, bool createParameterName, bool createColumnName, string appendQuery,string suffix)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            List<string> columns = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columns.Add(dt.Columns[i].ColumnName);
                dic.Add(i, TYPE.GetDBTypeName(dt.Columns[i].DataType));
            }
            List<string> queries = new List<string>();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            int len = dt.Columns.Count;
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                string st = string.Join("", SeprateChar , UniqueService.CreateNewDouble() , suffix);
                List<string> values = new List<string>();
                foreach (var item in dic)
                {
                    string val = string.Join("", "@" , dt.Columns[item.Key].ColumnName , st, r);
                    values.Add(val);
                    parameters.Add(new SQLiteParameter(val, TYPE.Set(dt.Rows[r][item.Key], item.Value)));
                }
                queries.Add( string.Join("", query , (createColumnName ? " (" + string.Join(" , ", columns) + ") " : " ") , (createParameterName ? " VALUES(" + string.Join(" , ", values) + ") " : " ") , appendQuery , "; "));
            }
            return new KeyValuePair<string, SQLiteParameter[]>(string.Join("", queries), parameters.ToArray());
        }
        public string CreateQuery(string query, bool createParameterName, bool createColumnName, string appendQuery, params SQLiteParameter[] parameters)
        {
            if (createColumnName)
            {
                List<string> queries = new List<string>();
                for (int i = 0; i < parameters.Length; i++)
                    queries.Add(parameters[i].ParameterName.Split(SeprateChar).First().Replace("@", ""));
                query += string.Join(""," ( ", string.Join(" , ", queries) , " )");
            }
            if (createParameterName)
            {
                List<string> queries = new List<string>();
                for (int i = 0; i < parameters.Length; i++)
                    queries.Add(parameters[i].ParameterName);
                query += string.Join(""," VALUES( ", string.Join(" , ", queries) , " )");
            }
            query += appendQuery;
            return query;
        }

        #endregion

        #region Ready Query
        public DataTable GetDataTable(string tableName)
        {
            Connection.Close();
            Connection.Open();
                    ExecuteStart(this, "");
            SQLiteTransaction transaction = Connection.BeginTransaction();
            DataTable dt = new DataTable();
            try
            {
                Command = Connection.CreateCommand();
                Command.CommandText = string.Format("SELECT * FROM {0}", tableName);
                Adapter = new SQLiteDataAdapter(Command);
                Adapter.AcceptChangesDuringFill = false;
                Adapter.Fill(dt);
                dt.TableName = tableName;
                foreach (DataRow row in dt.Rows)
                    row.AcceptChanges();
                return dt;
            }
            catch (Exception Ex)
            {
                if (ShowException) System.Windows.MessageBox.Show(Ex.Message);
                ExecuteError(this,"", Ex);
                return dt;
            }
            finally
            {
                transaction.Commit();
                Connection.Close();
                ExecuteFinish(this, "", Result);
            }
        }
        public void SaveDataTable(DataTable dt)
        {
            Connection.Clone();
            Connection.Open();
                    ExecuteStart(this, "");
            SQLiteTransaction transaction = Connection.BeginTransaction();
            try
            {
                Command = Connection.CreateCommand();
                Command.CommandText = string.Format("SELECT * FROM {0}", dt.TableName);
                Adapter = new SQLiteDataAdapter(Command);
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(Adapter);
                Adapter.Update(dt);
            }
            catch (Exception Ex)
            {
               if(ShowException) System.Windows.MessageBox.Show(Ex.Message);
                ExecuteError(this, "", Ex);
            }
            finally
            {
                transaction.Commit();
                Connection.Close();
                ExecuteFinish(this, "", Result);
            }
        }

        public void Drop(string tableName)
        {
            Execute("DROP TABLE IF EXISTS " + tableName, ExecuteMode.ExecuteNonQuery);
        }
        public object Create(string tableName, Dictionary<string, string> columnDic, string attachment = "")
        {
            return Create(tableName, CollectionService.GetAllItems(columnDic," "," , "), attachment);
        }
        public  object Create(string tableName, string columns, string attachment = "")
        {
            string query = string.Join("", @"
            CREATE TABLE IF NOT EXISTS ",
                tableName, " (", columns, ") ", attachment, ";");
            return Execute(query, ExecuteMode.ExecuteNonQuery);
        }

        public bool Exist(string tableName,string column,string value)
        {
            return Exist(tableName, string.Join("", "WHERE " ,column," = " , value));
        }
        public bool Exist(string tableName,string condition)
        {
            return Select(tableName,condition).Rows.Count > 0;
        }
        public bool IsEmpty(string tableName)
        {
            return Count(tableName) <= 0;
        }
        public double Count(string tableName)
        {
            return Convert.ToDouble(Execute("SELECT Count(*) FROM " + tableName, ExecuteMode.ExecuteScalar));
        }

        public DataTable Select(string tableName, string condition, params string[] columns)
        {
            string col = "";
            if (columns != null && columns.Length > 0) col = string.Join(" ,", columns);
            else col = "*";
            return Select(string.Join("", @"SELECT " , col , " FROM ", tableName , " ", condition , " ;"));
        }
        public DataTable Select(string tableName, double id)
        {
            return Select(tableName, " WHERE ID = " + id);
        }
        public DataTable Select(string query)
        {
            try
            {
                Adapter = new SQLiteDataAdapter(query, Connection);
                var cmdBuilder = new SQLiteCommandBuilder(Adapter);
                DataTable dt = new DataTable();
                Adapter.Fill(dt);
                IsChanged = false;
                ExecuteFinish(this, query, dt);
                return dt;
            }
            catch
            {
                return (DataTable)Execute(query, ExecuteMode.ExecuteReader);
            }
        }
        public DataTable SelectFirst(string tableName)
        {
            return Select(tableName, "WHERE RecNo = (SELECT MIN(RecNo) FROM" + tableName + ")");
        }
        public DataTable SelectLast(string tableName)
        {
            return Select(tableName, "WHERE RecNo = (SELECT MAX(RecNo) FROM" + tableName + ")");
        }

        public object Insert(string tableName ,string[] columnNames,object[] objects, string condition)
        {
            SQLiteParameter[] parameters = new SQLiteParameter[Math.Min(columnNames.Length, objects.Length)];
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = new SQLiteParameter("@" + columnNames[i],objects[i]);
            return Insert(tableName,condition,parameters);
        }
        public object Insert(DataTable dt, string condition = "",bool continueUpdateOnError = true)
        {
            string tableName = dt.TableName;
            Connection.Close();
            ExecuteStart(this, "");
            int r = -1;
            try
            {
                Adapter = new SQLiteDataAdapter("SELECT * FROM " + tableName, Connection);
                var cmdBuilder = new SQLiteCommandBuilder(Adapter);
                Adapter.UpdateCommand = cmdBuilder.GetUpdateCommand(true);
                Adapter.InsertCommand = cmdBuilder.GetInsertCommand(true);
                Adapter.ContinueUpdateOnError = continueUpdateOnError;
                r = Adapter.Update(dt);
                IsChanged = true;
                if (Convert.ToInt32(r) < 1) throw new Exception("Not update any record!");
                ExecuteFinish(this, Adapter.UpdateCommand.CommandText, r);
            }
            catch(Exception ex1)
            {
                if (continueUpdateOnError)
                    try { Insert(tableName, dt, condition); }
                    catch (System.Exception ex2)
                    {
                        ExecuteError(this, "INSERT OR REPLACE ", ex2);
                        if (ShowException) throw new System.Exception(ex2.Message);
                    }
                else
                {
                    ExecuteError(this, "INSERT OR REPLACE ", ex1);
                    if (ShowException) throw new System.Exception(ex1.Message);
                }
            }
            return r;
        }
        public object Insert(string tableName, DataTable dt, string condition = "")
        {
            try
            {
                string query = string.Join("", @"INSERT OR REPLACE INTO " ,tableName , " ");
                KeyValuePair<string, SQLiteParameter[]> q = CreateQuery<KeyValuePair<string, SQLiteParameter[]>>(query, dt, true, true, condition, "");
                return Execute(q.Key, ExecuteMode.ExecuteNonQuery, q.Value);
            }
            finally { ExecuteFinish(this, "INSERT OR REPLACE ", Result); }
            //Connection.Close();
            //ExecuteStart(this, "");
            //int r = -1;
            //try
            //{
            //    Adapter = new SQLiteDataAdapter("SELECT * FROM " + tableName, Connection);
            //    var cmdBuilder = new SQLiteCommandBuilder(Adapter);
            //    Adapter.InsertCommand = cmdBuilder.GetInsertCommand();
            //    Adapter.InsertCommand.CommandText = "INSERT OR REPLACE " + Adapter.InsertCommand.CommandText.Substring(6);
            //    Adapter.ContinueUpdateOnError = true;
            //    r = Adapter.Update(dt);
            //    IsChanged = true;
            //    ExecuteFinish(this, "", r);
            //}
            //catch (System.Exception ex)
            //{
            //    ExecuteError(this, "", ex);
            //    if (ShowException) throw new System.Exception(ex.Message);
            //}
            //finally
            //{
            //    ExecuteFinish(this, "", Result);
            //}
            //try
            //{
            //    Adapter = new SQLiteDataAdapter("SELECT * FROM " + dt.TableName, Connection);
            //    var cmdBuilder = new SQLiteCommandBuilder(Adapter);
            //    Adapter.UpdateCommand = cmdBuilder.GetUpdateCommand();
            //    string query = Adapter.InsertCommand.CommandText;
            //    string[] stra = query.Split(new string[] { ") VALUES (" }, StringSplitOptions.None);
            //    List<string> vals = stra.Last().Split(new string[] { ", ", ",", ")" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    List<string> cols = (stra = MiMFa_StringService.FirstFindAndSplit(stra.First(), " ([")).Last().Split(new string[] { "], [", "]" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    int len = Math.Min(cols.Count, vals.Count);
            //    for (int i = 0; i < len; i++)
            //        if (!dt.Columns.Contains(cols[i]))
            //            dt.Columns.Add(cols[i], typeof(object));
            //    Adapter.InsertCommand.CommandText += condition;
            //    //Adapter.ContinueUpdateOnError = true;
            //    r = Adapter.Update(dt);
            //    IsChanged = true;
            //    ExecuteFinish(this, Adapter.InsertCommand.CommandText, r);
            //}
            //catch (Exception ex1)
            //{
            //    try { Insert(dt.TableName, dt, condition); }
            //    catch (System.Exception ex)
            //    {
            //        ExecuteError(this, "INSERT OR REPLACE ", ex);
            //        if (ShowException) throw new System.Exception(ex.Message);
            //    }
            //    finally
            //    {
            //        ExecuteFinish(this, "INSERT OR REPLACE ", Result);
            //    }
            //}
            //return r;
        }
        public object Insert(string tableName, DataRow dr)
        {
            string query = string.Join("", @"INSERT OR REPLACE INTO " , tableName, " ");
            return Execute(query, ExecuteMode.ExecuteNonQuery, true,false, dr.ItemArray);
        }
        public object Insert(string tableName, object[] oa)
        {
            string query = string.Join("", @"INSERT OR REPLACE INTO " , tableName , " ");
            return Execute(query, ExecuteMode.ExecuteNonQuery, true,false, oa);
        }
        public object Insert<T>(string tableName, Dictionary<string, string> columnDic, string condition,params T[] obj)
        {
            return Execute(@"INSERT OR REPLACE INTO " + tableName, ExecuteMode.ExecuteNonQuery, columnDic, true,true,condition,obj);
        }
        public object Insert(string tableName, Dictionary<string, string> columnDic, object obj, string condition = "")
        {
            string query = @"
            INSERT OR REPLACE INTO " + tableName;
            Dictionary<string, object> sodic = new Dictionary<string, object>();
            foreach (var item in columnDic)
            {
                object o = null;
                PropertyInfo[] pi = InfoService.GetProperties(obj, item.Key, true);
                if (pi.Length > 0) o = pi.First().GetValue(obj);
                else {
                    FieldInfo[] fi = InfoService.GetFields(obj, item.Key, true);
                    if (fi.Length > 0) o = fi.First().GetValue(obj);
                }
                sodic.Add(item.Key, TYPE.Set(o,item.Value));
            }
            return Execute(query, ExecuteMode.ExecuteNonQuery,true,true, sodic);
        }
        public object Insert(string tableName, string columns, string values, string condition = "",SQLiteParameter[] parameters = null)
        {
            string query = string.Join("", @"
            INSERT OR REPLACE INTO " , tableName , " (" , columns , ") VALUES(" , values , ") " , ((condition != null) ? condition : "") ,";");
            return Execute(query, ExecuteMode.ExecuteNonQuery,false, false, parameters);
        }
        public object Insert(string tableName,string condition = "", SQLiteParameter[] parameters = null)
        {
            string query = @"
            INSERT OR REPLACE INTO " + tableName ;
            return Execute(query, ExecuteMode.ExecuteNonQuery,true,true,parameters);
        }
        public object Insert(string tableName, SQLiteParameterCollection parameters = null,string condition = "")
        {
            string query = @"
            INSERT OR REPLACE INTO " + tableName ;
            return Execute(query, ExecuteMode.ExecuteNonQuery,true,true,parameters);
        }
        public object Insert(string tableName, string columns, string values, string condition = "",SQLiteParameterCollection parameters = null)
        {
            string query = string.Join("", @"
            INSERT OR REPLACE INTO " , tableName ," (" , columns , ") VALUES(" , values , ") " , ((condition != null) ? condition:"") ,";");
            return Execute(query, ExecuteMode.ExecuteNonQuery,parameters);
        }

        public object Delete(string tableName, string condition = "")
        {
            string query = string.Join("", @"
            DELETE FROM " ,
                tableName , " " , condition , " ;");
            return Execute(query, ExecuteMode.ExecuteNonQuery);
        }

        public object GetColumn(string tableName, string columnName, string condition = null)
        {
            return Execute(string.Join("", @"SELECT " , columnName , " FROM " , tableName , " " , condition , " ;"), ExecuteMode.ExecuteReader);
        }

        public void CreateHeavyData(string tableName, string valueType = "BLOB")
        {
            Create(tableName, string.Join("", @"
                        FromTable TEXT NOT NULL,
                        FromColumn TEXT NOT NULL,
                        FromRecord TEXT NOT NULL,
                        Value " , valueType ,
                        ", ValueType TEXT, PRIMARY KEY (FromTable, FromColumn, FromRecord)"));
        }
        public object GetFromHeavyData(string tableName, string FromTable, string fromColumn, string FromRecord)
        {
            DataTable dt = (DataTable)Execute(string.Join("", @"SELECT * FROM " , tableName , " WHERE FromTable = '" , FromTable , "' AND FromColumn = '" , fromColumn , "' AND FromRecord = '" , FromRecord , "' ; "), ExecuteMode.ExecuteReader);
            if (dt.Rows.Count > 0)
                return TYPE.Get(dt.Rows[0]["Value"], dt.Rows[0]["ValueType"].ToString());
            return null;
        }
        public object SetToHeavyData(string tableName, string FromTable, string fromColumn, string FromRecord,object value, string condition = "")
        {
            return Execute(string.Join("", @"INSERT OR REPLACE INTO " , tableName , " VALUES ('" , FromTable , "','" , fromColumn , "','" , FromRecord , "', @Value,'", ((value != null)?value.GetType().Name : "Null" ),"') " , condition , " ;"), ExecuteMode.ExecuteNonQuery, new SQLiteParameter("@Value", TYPE.Set(value,TYPE.BLOB)));
        }
        public object DeleteFromHeavyData(string tableName, string FromTable, string fromColumn, string FromRecord)
        {
            string query = string.Join("", @"DELETE FROM " , tableName , " WHERE FromTable = '" , FromTable ,"' AND FromColumn = '" , fromColumn, "' AND FromRecord = '" , FromRecord , "' ; ");
            return Execute(query, ExecuteMode.ExecuteNonQuery);
        }

        public void CreateDic(string tableName, string valueType = "TEXT")
        {
            Create(tableName, @"Sign TEXT PRIMARY KEY NOT NULL, Value " + valueType);
        }
        public Dictionary<string, T> GetDic<T>(string tableName, string condition = "")
        {
            DataTable dt = (DataTable)Execute(
               string.Join("", @"SELECT * FROM " ,
                tableName , " " , condition , " ;"), ExecuteMode.ExecuteReader);
            Dictionary<string, T> dic = new Dictionary<string, T>();
            foreach (DataRow item in dt.Rows)
                dic.Add(item[0].ToString(), (T)item[1]);
            return dic;
        }
        public  object[] GetKeys(string tableName, string value, string condition = "", bool blobValue = false)
        {
            DataTable dt = (DataTable)Execute(string.Join("", @"SELECT * FROM " ,
                tableName , " WHERE Value = '" , value ,"' " , condition , " ;"), ExecuteMode.ExecuteReader);
            object[] oa = new object[dt.Rows.Count];
            for (int i = 0; i < oa.Length; i++)
                oa[i] = TYPE.Get(dt.Rows[i]["Sign"]);
            return oa;
        }
        public  object GetLikeValue(string tableName, string key, string condition, bool blobValue = false)
        {
            DataTable dt = (DataTable)Execute(string.Join("", @"SELECT * FROM " ,
                tableName , " WHERE Sign Like '" , key, "' " , condition, " ;"), ExecuteMode.ExecuteReader);
            if (dt.Rows.Count > 0)
                if (blobValue) return IOService.Deserialize((byte[])dt.Rows[0]["Value"]);
                else return dt.Rows[0][1];
            return null;
        }
        public object InsertValues(string tableName, Dictionary<string, object> columnValues, string condition)
        {
            string keys = "([Sign],[Value])";
            List<string> values = new List<string>();
            List<SQLiteParameter> sqls = new List<SQLiteParameter>();
            int i = 0;
            foreach (var item in columnValues)
            {
                values.Add( string.Join("", " ('" , item.Key , "', @Value" , i, ") "));
                object value = IOService.Serialize(item.Value);
                sqls.Add(new SQLiteParameter("@Value" + i, value));
                i++;
            }
            return Execute(string.Join("", @"INSERT OR REPLACE INTO " , tableName ," ",keys , " VALUES ", string.Join(",", values), " " , condition , " ;"), ExecuteMode.ExecuteNonQuery, sqls.ToArray());
        }
        public object InsertValue(string tableName, string key, object value, string condition, bool blobValue = false)
        {
            if (blobValue)
                value = IOService.Serialize(value);
            return Execute(string.Join("", @"INSERT OR REPLACE INTO " , tableName, " VALUES ('" , key , "', @Value) " , condition , " ;"), ExecuteMode.ExecuteNonQuery, new SQLiteParameter("@Value", value));
        }
        public  object GetValue(string tableName, string key, string condition, bool blobValue = false)
        {
            DataTable dt = (DataTable)Execute(string.Join("", @"SELECT * FROM ", tableName , " WHERE Sign = '", key , "' ", condition , " ;"), ExecuteMode.ExecuteReader);
            if (dt.Rows.Count > 0)
                if (blobValue && dt.Rows[0]["Value"] is byte[]) return IOService.Deserialize((byte[])dt.Rows[0]["Value"]);
                else return dt.Rows[0]["Value"];
            return null;
        }
        public  object GetValue(string tableName, string key, bool blobValue = false)
        {
            return GetValue(tableName, key, "", blobValue);
        }
        public  object SetValue(string tableName, string key, object value, bool blobValue = false)
        {
            return SetValue(tableName, key, value, "", blobValue);
        }
        public object SetValue(string tableName, string key, object value, string condition, bool blobValue = false)
        {
            if (blobValue)
                value = IOService.Serialize(value);
            return Execute(string.Join("", @"INSERT OR REPLACE INTO " ,tableName , " VALUES ('" , key , "', @Value) " , condition , " ;"), ExecuteMode.ExecuteNonQuery, new SQLiteParameter("@Value", value));
        }
        public object SetValues(string tableName, Dictionary<string, string> columnValues, string condition)
        {
            List<string> sets = new List<string>();
            foreach (var item in columnValues)
                sets.Add( string.Join("", "[" , item.Key , "] = '", item.Value, "' "));
            return Execute(string.Join("", @"UPDATE " , tableName ,
                " SET ",string.Join(" , ", sets)," " , condition , " ;"), ExecuteMode.ExecuteNonQuery);
        }
        public object SetValues(string tableName, Dictionary<string, object> columnValues, string condition)
        {
            List<string> sets = new List<string>();
            List<SQLiteParameter> sqls = new List<SQLiteParameter>();
            int i = 0;
            foreach (var item in columnValues)
            {
                object value = IOService.Serialize(item.Value);
                sets.Add( string.Join("", "[" , item.Key , "] = @Value",i," "));
                sqls.Add(new SQLiteParameter("@Value" + i, value));
                i++;
            }
            return Execute(string.Join("", @"UPDATE " , tableName , " SET ", string.Join(" , ", sets)," " , condition , " ;"), ExecuteMode.ExecuteNonQuery, sqls.ToArray());
        }
        public object SetValue(string tableName, KeyValuePair<string, string> columnValue, string condition, bool blobValue = false)
        {
            object value = columnValue.Value;
            if (blobValue)
                value = IOService.Serialize(value);
            return Execute(string.Join("", @"UPDATE " , tableName, " SET [" , columnValue.Key , "] = @Value " , condition , " ;"), ExecuteMode.ExecuteNonQuery, new SQLiteParameter("@Value", value));
        }
        #endregion

        #region Private
        private string _TempDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\Temp\SQLite\";
        #endregion
    }

    #region Other
    [Serializable]
    public class SQLiteDataBaseTypes : IDataBaseTypes
    {
        public string INT { get; set; } = "INT";
        public string INTEGER { get; set; } = "INTEGER";
        public string TINYINT { get; set; } = "INTEGER";
        public string SMALLINT { get; set; } = "SMALLINT";
        public string MEDIUMINT { get; set; } = "INTEGER";
        public string BIGINT { get; set; } = "BIGINT";
        public string UNSIGNEDBIGINT { get; set; } = "INTEGER";
        public string INT2 { get; set; } = "INTEGER";
        public string INT8 { get; set; } = "INT64";

        public string CHARACTER { get; set; } = "CHAR";
        public string VARCHAR { get; set; } = "VARCHAR";
        public string VARYINGCHARACTER { get; set; } = "TEXT";
        public string NCHAR { get; set; } = "NCHAR";
        public string NVARCHAR { get; set; } = "NVARCHAR";
        public string TEXT { get; set; } = "TEXT";
        public string NTEXT { get; set; } = "NTEXT";
        public string CLOB { get; set; } = "CLOB";

        public string BLOB { get; set; } = "BLOB";
        public string OBJECT { get; set; } = "BLOB";
        public string IMAGE { get; set; } = "IMAGE";

        public string REAL { get; set; } = "REAL";
        public string DOUBLE { get; set; } = "DOUBLE";
        public string DECIMAL { get; set; } = "DECIMAL";
        public string DOUBLE_PRECISION { get; set; } = "DOUBLE PRECISION";
        public string FLOAT { get; set; } = "FLOAT";

        public string NUMERIC { get; set; } = "NUMERIC";
        public string BOOL { get; set; } = "BOOL";
        public string BOOLEAN { get; set; } = "BOOLEAN";
        public string DATE { get; set; } = "DATE";
        public string TIME { get; set; } = "TIME";
        public string DATETIME { get; set; } = "DATETIME";

        public string GetDBTypeName(Type type)
        {try
            {
                string tn = type.Name.ToLower();
                if (type.IsAssignableFrom(typeof(string)))
                    return TEXT;
                if (type.IsAssignableFrom(typeof(Image)) || type.IsAssignableFrom(typeof(Bitmap)))
                    return IMAGE;
                if (type == typeof(UInt64) || type == typeof(Int64))
                    return DOUBLE;
            }
            catch { }
            return OBJECT;
        }
        public string GetDBTypeName( object obj)
        {try
            {
                Type type = obj.GetType();
                string tn = type.Name.ToLower();

                if ((type is IDictionary || tn.StartsWith("dictionary") || tn.StartsWith("mimfa_dictionary")))
                    return OBJECT;
                if (tn.StartsWith("mimfa_matrix"))
                    return OBJECT;
                if (tn.StartsWith("keyvaluepair"))
                    return OBJECT;
                if ((type is IList || tn.StartsWith("list") || tn.StartsWith("mimfa_list")))
                    return OBJECT;
                if (type == typeof(byte[]))
                    return BLOB;
                if (tn.EndsWith("[]"))
                    return OBJECT;
                if (type is ICollection)
                    return OBJECT;
                if (tn.StartsWith("stack"))
                    return OBJECT;
                if (tn.StartsWith("queue"))
                    return OBJECT;
                if (tn.StartsWith("datatable"))
                    return OBJECT;
                if (tn.StartsWith("datarow"))
                    return OBJECT;
                if (obj is Enum)
                    return TEXT;
                if (obj is Bitmap)
                    return IMAGE;
                if (obj is Uri)
                    return OBJECT;
                if (obj is byte)
                    return BLOB;
                if (obj is String)
                    return TEXT;
                if (double.NaN.Equals(obj))
                    return DOUBLE;
                if (type == typeof(UInt64) || type == typeof(Int64))
                    return DOUBLE;
            }
            catch { }
            return OBJECT;
        }
        public object Set(object obj, string type= null)
        {
            if (obj == null) return null;
                if (string.IsNullOrEmpty(type) && obj.GetType() == typeof(byte[])) type = "BLOB";
                if (string.IsNullOrEmpty(type)) type = obj.GetType().Name;
            type = type.Split(' ').First().ToUpper();
            Object o = DBNull.Value;
            switch (type)
            {
                case "BLOB":
                    return (obj == null || obj == o) ? new byte[]{ } : 
                        (obj.GetType().IsAssignableFrom(typeof(Image))|| obj.GetType().IsAssignableFrom(typeof(Bitmap))) ?
                            ConvertService.ToByteArray((Bitmap)obj) :
                            IOService.Serialize(obj);
                case "IMAGE":
                    return (obj == null) ? null : ConvertService.ToByteArray((Image)obj);
                case "BITMAP":
                    return (obj == null) ? null : ConvertService.ToByteArray((Bitmap)obj);
                default:
                    return obj;
            }
        }
        public object Get(object obj, string type = null)
        {
            //if (obj == null && type == TEXT) return "";
            //if (obj == null && 
            //    (type == INTEGER 
            //    || type == REAL 
            //    || type == NUMERIC) ) return 0;
            //if (obj == null &&
            //    (type == BOOLEAN)) return false;
            if (obj == null) return obj;
            if (string.IsNullOrEmpty(type) && obj.GetType() == typeof(byte[])) type = "BLOB";
            if (string.IsNullOrEmpty(type)) type = obj.GetType().Name;
            if (obj.GetType() == typeof(DBNull)) return null;
            type = type.ToUpper();
            try
            {
                switch (type)
                {
                    case "DBNULL":
                        return null;
                    case "BLOB":
                        if (obj == null) return null;
                        else try { return IOService.Deserialize((byte[])obj); }
                            catch
                            {
                                return ConvertService.ToImage((byte[])obj);
                            }
                    case "BYTE[]":
                        if (obj == null) return null;
                        else try { return IOService.Deserialize((byte[])obj); }
                            catch
                            {
                                return ConvertService.ToImage((byte[])obj);
                            }
                    case "IMAGE":
                        return (obj == null) ? null : ConvertService.ToImage((byte[])obj);
                    case "BITMAP":
                        return (obj == null) ? null : ConvertService.ToImage((byte[])obj);
                    default:
                        return obj;
                }
            }
            catch { return obj; }
        }
    }
    #endregion

}
