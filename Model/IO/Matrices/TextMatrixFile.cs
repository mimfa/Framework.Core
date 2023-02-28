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
    public class TextMatrixFile : MatrixFile<string>
    {
        public override string ToParameter(string str) => str ?? DefaultParameter;
        public override string FromParameter(string param) => param?? DefaultString;


        public TextMatrixFile(ChainedFile document, string dp = "", string ds = "")
        : base(document,dp,ds)
        {
        }
    }
}
