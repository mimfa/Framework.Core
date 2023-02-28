using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.Script
{
    [Serializable]
    public class ScriptException : Exception
    {
        public ScriptException()
            : base("Script Exception")
        {
        }

        public ScriptException(string message)
            : base(message)
        {
        }

        public ScriptException(Exception innerException)
            : base(null, innerException)
        {
        }

        public ScriptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ScriptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string Description { get; internal set; }
        public int Line { get; internal set; }
        public int Column { get; internal set; }
        public int Number { get; internal set; }
        public string Text { get; internal set; }
    }
}
