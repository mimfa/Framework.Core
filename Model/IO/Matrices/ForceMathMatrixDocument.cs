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
    public class ForceMathMatrixDocument : MatrixDocument<double>
    {
        public override double ToParameter(string str) => str == null ? DefaultParameter : ConvertService.ForceToDouble(str);
        public override string FromParameter(double param) => param.ToString();


        public ForceMathMatrixDocument(ChainedDocument document, double dp = 0, string ds = "")
        : base(document,dp,ds)
        {
        }
    }
}
