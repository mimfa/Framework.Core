using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Translate
{
    public interface ITranslatable
    {
        string Translate(string text);
    }
}
