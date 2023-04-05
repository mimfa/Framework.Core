using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiMFa.Engine.Template;
using MiMFa.Engine.Translate;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.General;
using MiMFa.Service;

namespace MiMFa
{
    public static class Default
    {
        public static DateTime SystemDateTime => System.DateTime.UtcNow;

        public static SmartDateTime DateTime { get; set; } = new SmartDateTime(TimeZoneMode.IranStandard);
        public static SmartDate Date { get { return DateTime.GetSmartDate(); } }
        public static SmartTime Time { get { return DateTime.GetSmartTime(); } }
        public static bool RightToLeft
        {
            get => HasTranslator ? Translator.RightToLeft : _RightToLeft;
            set
            {
                if (HasTranslator)
                    Translator.RightToLeft = value;
                _RightToLeft = value;
            }
        }
        public static bool _RightToLeft = false;
        public static bool HasTranslator => Translator!= null;
        public static ITranslator Translator { get; set; } = null;
        public static bool HasTemplator => Templator != null;
        public static ITemplator Templator { get; set; } = null;

        public static void Normalize(Control control, int nest = 15, bool toolstrip = true, params object[] exceptControls)
        {
            Order(control, nest , toolstrip,exceptControls);
            Template(control, nest , toolstrip,exceptControls);
            Translate(control, nest , toolstrip, exceptControls);
        }

        public static bool Order(Control control, int nest = 15, bool toolstrip = true, params object[] exceptControls)
        {
            if (HasTemplator)
            {
                ControlService.SetControlsTabIndices(control, nest,1, toolstrip, exceptControls);
                return true;
            }
            return false;
        }
        public static bool Template(Control control, int nest = 15, bool toolstrip = true, params object[] exceptControls)
        {
            if (HasTemplator)
            {
                Templator.Update(control, nest, toolstrip, exceptControls);
                return true;
            }
                return false;
        }
        public static string Translate(string text)
        {
            if (HasTranslator) return Translator.Get(text);
            return text;
        }
        public static bool Translate(Control control, int nest = 15, bool toolstrip = true, params object[] exceptControls)
        {
            if (HasTranslator)
            {
                Translator.Update(control, nest, toolstrip, exceptControls);
                return true;
            }
                return false;
        }
        public static string Translate(params string[] texts)
        {
            if (HasTranslator) return Translator.Get(texts);
            return string.Join("", texts);
        }

    }
}
