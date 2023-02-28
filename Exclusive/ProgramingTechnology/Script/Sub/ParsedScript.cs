using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.Script
{
    public sealed class ParsedScript : IDisposable
    {
        private object _dispatch;
        private readonly ScriptEngine _engine;

        internal ParsedScript(ScriptEngine engine, IntPtr dispatch)
        {
            _engine = engine;
            _dispatch = Marshal.GetObjectForIUnknown(dispatch);
        }

        public object CallMethod(string methodName, params object[] arguments)
        {
            if (_dispatch == null)
                throw new InvalidOperationException();

            if (methodName == null)
                throw new ArgumentNullException("methodName");

            try
            {
                return _dispatch.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, _dispatch, arguments);
            }
            catch
            {
                if (_engine.Site.LastException != null)
                    throw _engine.Site.LastException;

                throw;
            }
        }

        void IDisposable.Dispose()
        {
            if (_dispatch != null)
            {
                Marshal.ReleaseComObject(_dispatch);
                _dispatch = null;
            }
        }
    }
}
