using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using MiMFa.General;
using System.Xml;
using MiMFa.Model.IO.Filter;
using Microsoft.Office.Interop.Excel;
namespace MiMFa.Model.IO
{
    [Serializable]
    public class ChainedDocumentCollection : Dictionary<string,ChainedDocument>
    {
        public ChainedDocument this[int index] { get => this[Keys.ElementAt(index)]; set => this[Keys.ElementAt(index)] = value; }
        
        public void Add(KeyValuePair<string, ChainedDocument> item)
        {
            if (ContainsKey(item.Key)) base[item.Key].AppendForePiece(item.Value);
            else base.Add(item.Key, item.Value);
        }
        public new void Add(string key, ChainedDocument value)
        {
           if(ContainsKey(key)) base[key].AppendForePiece(value);
           else base.Add(key,value);
        }
        public void Add(ChainedDocument value)
        {
            Add(value.Directory, value);
        }
    }
}
