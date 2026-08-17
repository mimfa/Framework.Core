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

namespace MiMFa.Model.IO.Matrices
{
    [Serializable]
    public class TextMatrixDocument : MatrixDocument<string>
    {
        public override string ToParameter(string str) => str ?? DefaultParameter;
        public override string FromParameter(string param) => param?? DefaultString;


        public TextMatrixDocument(ChainedDocument document, string dp = "", string ds = "")
        : base(document,dp,ds)
        {
        }
    }
}
