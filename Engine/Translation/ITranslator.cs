using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Engine.Translate
{
    public interface ITranslator
    {
        string Lang { get; set; }
        string Language { get; set; }
        string CharSet { get; set; }
        bool RightToLeft { get; set; }

        ITranslator Load();
        ITranslator Load(string path, string name = null);

        ITranslator Update(Control mainControl, int nest = 10, bool toolstrip = true, params object[] exceptControls);

        object Get(object key);
        string Get(string key);
        string Get(params string[] keys);
        IEnumerable<string> GetLanguagesPath();
        IEnumerable<string> GetLanguagesName();
        bool Set(string key, string value);
    }
}
