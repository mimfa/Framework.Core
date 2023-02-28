using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MiMFa.Service;

namespace MiMFa.General
{
    public class QueryToDataSet
    {
        public static DataSet DS;
        public static string DefaultTableName;
        private static string address;

        public static void Start(string connectionString, string AddDefaultTableName = null, params string[] columnsname)
        {
            DS = new DataSet();
            DefaultTableName = "Table";
            address = null;
            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            address = connectionString;
            DS = new DataSet();
            if (AddDefaultTableName != null)
            {
                CREATE(AddDefaultTableName, columnsname);
                DefaultTableName = AddDefaultTableName;
            }
        }

        //AUTO QUERY FUNCTION
        public static void ADD(DataTable Table, string Into_Table = null)
        {
            if (Into_Table == null) Into_Table = DefaultTableName;
            DS.Tables.Remove(Into_Table);
            Table.TableName = Into_Table;
            DS.Tables.Add(Table);

            //for (int i = 0; i < Table.Columns.Count; i++)
            //    DS.Tables[Into_Table].Columns[i].DataType = Table.Columns[i].DataType;

            //DS.Tables[Into_Table].Columns.Clear();
            //for (int i = 0; i < Table.Columns.Count; i++)
            //    DS.Tables[Into_Table].Columns.Add(Table.Columns[i].ToString(), Table.Columns[i].DataType);
            //DS.Tables[Into_Table].Rows.Clear();
            //foreach (var item in Table.Rows)
            //    DS.Tables[Into_Table].Rows.Add(item);
        }
        public static void ADD(string Table = null, string Into_Table = null)
        {
            if (Table == null) Table = DefaultTableName;
            ADD(DS.Tables[Table], Into_Table);
        }
        public static void CREATE(string TableName, params string[] ColumnsName)
        {
            DS.Tables.Add(TableName);
            foreach (var item in ColumnsName)
                DS.Tables[TableName].Columns.Add(item);
        }

        public static DataRow[] SELECT_ALL_FROM(string Table = null, string Where_Key_Is = null)
        {
            if (Table == null) Table = DefaultTableName;
            return DS.Tables[Table].Select();
        }
        public static DataRow[] SELECT_ALL_FROM(string Table = null)
        {
            if (Table == null) Table = DefaultTableName;
            return DS.Tables[Table].Select();
        }

        public static void INSERT_INTO(string Table = null, params DataRow[] Rows)
        {
            if (Table == null) Table = DefaultTableName;
            foreach (var item in Rows)
                DS.Tables[Table].Rows.Add(item);
        }
        public static void INSERT_INTO(string Table = null, params object[] Values)
        {
            if (Table == null) Table = DefaultTableName;
            DS.Tables[Table].Rows.Add(Values);
        }

        public static void UPDATE(string Table = null, int Where_Index = 0, string Equal_With = null, params object[] Values)
        {
            if (Table == null) Table = DefaultTableName;
            for (int i = 0; i < DS.Tables[Table].Rows.Count; i++)
                if (DS.Tables[Table].Rows[i][Where_Index].ToString() == Equal_With)
                    DS.Tables[Table].Rows[i].ItemArray = Values;
        }

        public static DataTable GET_PART_IN(DataTable Table, int Of_Index = 0, int To_Index = -1)
        {
            DataTable dt = Table.Clone();
            if (Table.Rows.Count <= To_Index) To_Index = Table.Rows.Count - 1;
            if (Of_Index > To_Index || Of_Index < 0) Of_Index = 0;
            for (int i = Of_Index; i <= To_Index; i++)
                dt.Rows.Add(Table.Rows[i].ItemArray);
            return dt;
        }
        public static DataTable GET_PART_IN(string Table = null, int Of_Index = 0, int To_Index = -1)
        {
            if (Table == null) Table = DefaultTableName;
            return GET_PART_IN(DS.Tables[Table], Of_Index, To_Index);
        }

        public static DataTable GET_SHAKED(DataTable Table)
        {
            int length = Table.Rows.Count;
            int[] randindex = CollectionService.RandIndex(length);
            DataTable dt = Table.Clone();

            for (int i = 0; i < length; i++)
                dt.Rows.Add(Table.Rows[randindex[i]].ItemArray);

            return dt;
        }
        public static DataTable GET_SHAKED(string Table = null)
        {
            if (Table == null) Table = DefaultTableName;
            return GET_SHAKED(DS.Tables[Table]);
        }

        public static void APPEND(DataTable Table, string ToThis = null, int IDIndex = 0)
        {
            if (ToThis == null) ToThis = DefaultTableName;
            int maxid = -99999999;
            for (int i = 0; i < DS.Tables[ToThis].Rows.Count; i++)
                maxid = Math.Max(maxid, Convert.ToInt32(DS.Tables[ToThis].Rows[i][IDIndex]));
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                Table.Rows[i][IDIndex] = ++maxid;
                DS.Tables[ToThis].Rows.Add(Table.Rows[i].ItemArray);
            }
        }

        public static void UPDATE(string Table = null, DataTable ToThis = null)
        {
            DELETE(Table);
            ADD(ToThis);
        }

        public static void UP(string Table = null, int Where_Index = 0)
        {
            if (Table == null) Table = DefaultTableName;
            if (Where_Index > 0)
            {
                DataRow drNow = DS.Tables[Table].NewRow();
                DataRow drPrev = DS.Tables[Table].NewRow();
                drPrev.ItemArray = DS.Tables[Table].Rows[Where_Index - 1].ItemArray;
                drNow.ItemArray = DS.Tables[Table].Rows[Where_Index].ItemArray;
                DataTable dt = DS.Tables[Table].Clone();
                while (DS.Tables[Table].Rows.Count > Where_Index + 1)
                {
                    dt.Rows.Add(DS.Tables[Table].Rows[Where_Index + 1].ItemArray);
                    DS.Tables[Table].Rows[Where_Index + 1].Delete();
                }
                DS.Tables[Table].Rows[Where_Index].Delete();
                DS.Tables[Table].Rows[Where_Index - 1].Delete();
                DS.Tables[Table].Rows.Add(drNow.ItemArray);
                DS.Tables[Table].Rows.Add(drPrev.ItemArray);
                for (int i = 0; i < dt.Rows.Count; i++)
                    DS.Tables[Table].Rows.Add(dt.Rows[i].ItemArray);
            }
        }
        public static void Down(string Table = null, int Where_Index = 0)
        {
            if (Table == null) Table = DefaultTableName;
            if (Where_Index < DS.Tables[Table].Rows.Count)
            {
                DataRow drNow = DS.Tables[Table].NewRow();
                DataRow drNext = DS.Tables[Table].NewRow();
                drNow.ItemArray = DS.Tables[Table].Rows[Where_Index].ItemArray;
                drNext.ItemArray = DS.Tables[Table].Rows[Where_Index + 1].ItemArray;
                DataTable dt = DS.Tables[Table].Clone();
                while (DS.Tables[Table].Rows.Count > Where_Index + 2)
                {
                    dt.Rows.Add(DS.Tables[Table].Rows[Where_Index + 2].ItemArray);
                    DS.Tables[Table].Rows[Where_Index + 2].Delete();
                }
                DS.Tables[Table].Rows[Where_Index + 1].Delete();
                DS.Tables[Table].Rows[Where_Index].Delete();
                DS.Tables[Table].Rows.Add(drNext.ItemArray);
                DS.Tables[Table].Rows.Add(drNow.ItemArray);
                for (int i = 0; i < dt.Rows.Count; i++)
                    DS.Tables[Table].Rows.Add(dt.Rows[i].ItemArray);
            }
        }

        public static void DELETE(string Table = null, int Where_Index = 0, string ID_Equal_With = null)
        {
            if (Table == null) Table = DefaultTableName;
            if (ID_Equal_With == null) DS.Tables[Table].Rows[Where_Index].Delete();
            else if (DS.Tables[Table].Rows[Where_Index]["ID"].ToString() == ID_Equal_With)
                DS.Tables[Table].Rows[Where_Index].Delete();
        }
        public static void DELETE(string Table = null)
        {
            if (Table == null) Table = DefaultTableName;
            DS.Tables.Remove(Table);
        }
        public static void TRUNCATE(string Table = null)
        {
            if (Table == null) Table = DefaultTableName;
            DS.Tables[Table].Rows.Clear();
        }

    }
}
