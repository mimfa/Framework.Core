using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.General
{
    public class SimplyInterpreter
    {
        public string Execute(string script)
        {
            if (string.IsNullOrWhiteSpace(script)) return null;
            string str = "";
            string[] commands = script.Split(new string[] { "@@@@@@@@@@","|||||" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in commands)
                try
                {
                    string com = item.Trim();
                    if (com.StartsWith("@PROCESS:"))
                        System.Diagnostics.Process.Start(com.Substring(9));
                    else if (com.StartsWith("@MESSAGE:"))
                        System.Windows.Forms.MessageBox.Show(com.Substring(9));
                    else if (com.StartsWith("@FILE:"))
                    {
                        string[] stra = com.Substring(6).Split(new string[] { "@VALUE:" }, StringSplitOptions.None);
                        System.IO.File.WriteAllText(stra[0], stra[1]);
                    }
                    else if (com.StartsWith("@EXIT:"))
                        Environment.Exit(MiMFa.Service.ConvertService.TryToInt(com.Substring(6),0));
                    else if (com.StartsWith("@EXECUTE:"))
                    {
                        foreach (var pcg in com.Substring(5).ToLower().Replace(" ","").Split(';'))
                            switch (pcg)
                            {
                                case "clearcnf":
                                case "clearcnffile":
                                case "clearconfig":
                                case "clearconfigfile":
                                case "clearconfigurationfile":
                                    if (File.Exists(Config.ConfigurationPath)) File.Delete(Config.ConfigurationPath);
                                    break;
                                case "clearcnfdir":
                                case "clearconfigdir":
                                case "clearconfigurationdirectory":
                                    if (Directory.Exists(Config.ConfigurationDirectory)) Directory.Delete(Config.ConfigurationDirectory);
                                    break;
                                default:
                                    break;
                            }
                    }
                    else if (com.StartsWith("@") && com.Length > 5 && com[4] == ':') str += com.Substring(5);
                    else str += item;
                }
                catch { }
            return str;
        }
    }
}
