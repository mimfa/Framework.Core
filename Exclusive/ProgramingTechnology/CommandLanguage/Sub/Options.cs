using MiMFa.General;
using MiMFa.Exclusive.ProgramingTechnology.ReportLanguage;
using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiMFa.Engine.Translate;

namespace MiMFa.Exclusive.ProgramingTechnology.CommandLanguage
{
    public class Options:TranslatableBase
    {

        public Options()
        {
        }
        
        public bool CaseSensivity { get; set; } = false;
        public bool Relax { get; set; } = false;
        public object FunctionResult { get; set; }
        public string UserName => Environment.UserName;
        public string MachineName => Environment.MachineName;
        public string UserDomainName => Environment.UserDomainName;
        public string IPv4 => NetService.GetInternalIPv4().ToString();
        public string MAC => NetService.GetMAC().ToString();

        public void UpdateTranslator(string key, string value)
        {
            if(Default.HasTranslator)
            Default.Translator.Set(key, value);
        }
    }
}
