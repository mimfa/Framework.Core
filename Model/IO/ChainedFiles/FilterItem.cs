using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.General;

namespace MiMFa.Model.IO.ChainedFiles
{
    public struct FilterItem
    {
        public int ID;
        public TableChangeMode Type;
        public long From;
        public long To;
        public List<string> Items;
        public bool IsCollection => From < To;
        public long ChangeNumber
        {
            get
            {
                int m = 0;
                switch (Type)
                {
                    case TableChangeMode.Insert:
                        m = 1;
                        break;
                    case TableChangeMode.Modify:
                        m = 0;
                        break;
                    case TableChangeMode.Delete:
                        m = -1;
                        break;
                    default:
                        m = 0;
                        break;
                }
                if (To < 0) 
                    if(Items.Count < 1) return m * int.MaxValue;
                    else return m * Items.Count;
                return m * (Math.Abs(To - From)+1);
            }
        }

        public FilterItem(TableChangeMode type, long fromIndex, long toIndex, List<string> items, int lastID)
        {
            ID = lastID;
            Type = type;
            Items = items;
            From = fromIndex;
            To = toIndex;
        }
        public FilterItem(FilterItem item, List<string> items, long fromIndex, long toIndex, int lastID)
            : this(item.Type, fromIndex, toIndex, items, lastID) { }
        public FilterItem(FilterItem item, long fromIndex, long toIndex, int lastID)
            : this(item.Type, fromIndex, toIndex, item.Items, lastID) { }
        public FilterItem(TableChangeMode type, long fromIndex, long toIndex, int lastID, string item = null)
            : this(type, fromIndex, toIndex, (item != null)?new List<string>(){ item }:new List<string>(), lastID) { }
        public FilterItem(TableChangeMode type, long index, int lastID, string item = null)
            : this(type, index, index, (item != null)?new List<string>(){ item }:new List<string>(), lastID) { }


        public override string ToString() => string.Join("",Type," ",IsCollection? (From + " to " + To) : From + ""," {",string.Join("; ",Items),"}");

        public string GetItem(long currectorInd)
        {
            if (From + Items.Count > currectorInd) return Items[Convert.ToInt32(currectorInd - From)];
            else if (Items.Count > 0) return Items.First();
            return null;
        }
    }
}
