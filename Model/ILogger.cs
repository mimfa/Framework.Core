using MiMFa.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    public interface ILogger
    {
        event General.GenericEventHandler<ILogger, MessageMode, string,string> Logging;
        event GenericEventListener<ILogger, MessageMode, string> Logged;

        void Subject(string message);
        void Success(string message);
        void Error(Exception ex);
        void Error(string message);
        void Message(string message);
        void Warning(string message);
        void Notice(string message);
        void Write(string message);
    }
}
