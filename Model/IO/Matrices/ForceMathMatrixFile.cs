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
using Microsoft.VisualBasic.Devices;
using MiMFa.General;
using System.Xml;
using MiMFa.Model.IO.ChainedFiles;
using Microsoft.Office.Interop.Excel;
using System.Windows.Shapes;

namespace MiMFa.Model.IO.Matrices
{
    [Serializable]
    public class ForceMathMatrixFile : MatrixFile<double>
    {
        double nd = 0;
        public override double ToParameter(string str) => str == null ? DefaultParameter : ConvertService.ForceToDouble(str);
        public override string FromParameter(double param) => param.ToString();


        public ForceMathMatrixFile(ChainedFile document, double dp = 0, string ds = "")
        : base(document,dp,ds)
        {
        }
    }
}
