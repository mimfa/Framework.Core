using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiMFa.Model.IO.Connector
{
    public class PlainTextFile : ConnectorBase
    {
        public PlainTextFile(string path, Encoding encoding = null) : base(InfoService.IsAddress(path, false)? path:null, encoding)
        {
            if (!string.IsNullOrEmpty(path) && path != Path) WriteText(path);
        }

        public override bool CreateNew()
        {
            PathService.CreateAllDirectories(System.IO.Path.GetDirectoryName(Path));
            if(!File.Exists(Path)) WriteText("");
            return base.CreateNew();
        }
        public override IEnumerable<string> ReadLines()
        {
            var lastLine = new List<string>();
            var sre = StartQuoteCharDetector;
            var ere = EndQuoteCharDetector;
            var lb = MetaLineBreakChar;
            foreach (var line in IOService.ReadLines(Path, Encoding))
                if (lastLine.Count == 0)
                {
                    var e = ere.Matches(line).Count;
                    var nl = ere.Replace(line,"");
                    if (sre.Matches(nl).Count > e)
                        lastLine.Add(line);
                    else yield return line;
                }
                else if (ere.Matches(line).Count > sre.Matches(line).Count)
                {
                    lastLine.Add(line);
                    yield return string.Join(lb, lastLine);
                    lastLine.Clear();
                }
                else lastLine.Add(line);

            if (lastLine.Count!= 0)
            {
                yield return string.Join(lb, lastLine);
                lastLine.Clear();
            }
        }
        public override string ReadText() => IOService.ReadText(Path, Encoding);
        public override int WriteAllLines(string[] lines)
        {
            File.WriteAllLines(Path,lines,Encoding);
            return lines.Length;
        }
        public override int WriteLines(IEnumerable<string> lines)
        {
            return IOService.WriteLines(Path,lines,Encoding);
        }
        public override int WriteText(string text)
        {
            IOService.WriteText(Path, text, Encoding);
            return -1;
        }

        public override int AppendLines(IEnumerable<string> lines)
        {
            return IOService.AppendLines(Path, lines, Encoding);
        }
        public override int AppendText(string text)
        {
            IOService.AppendText(Path, text, Encoding);
            return -1;
        }
    }
}
