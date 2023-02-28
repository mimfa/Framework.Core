using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Translate
{
    public abstract class TranslatableBase : ITranslatable
    {
        public virtual bool RightToLeft
        {
            get => HasTranslator ? Translator.RightToLeft : _RightToLeft;
            set
            {
                if (HasTranslator)
                    Translator.RightToLeft = value;
                _RightToLeft = value;
            }
        }
        public bool _RightToLeft = false;
        public virtual bool HasTranslator => Translator != null;
        public virtual ITranslator Translator { get; set; } = null;

        public virtual string Translate(string text)
        {
            if (HasTranslator) return Translator.Get(text);
            return Default.Translate(text);
        }
        public string Translate(params string[] texts)
        {
            if (HasTranslator) return Translator.Get(texts);
            return Default.Translate(texts);
        }
    }
}
