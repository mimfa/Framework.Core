using MiMFa.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Web
{
    public interface IWebBrowser<T>
    {
        event EventHandler Initialized;
        event GenericEventListener<T, string> Navigating;
        event GenericEventListener<T, string> Navigated;
        event GenericEventListener<T, string, int, object> DocumentCompleted;
        event GenericEventHandler<T, string, object, string> BeginDownload;
        event GenericEventHandler<T, string, int, string, object, bool> Downloading;
        event GenericEventListener<T, string, bool, string, object> FinishDownload;
        event GenericEventHandler<T, string, bool> WindowCreated;
        event GenericEventHandler<T, string, bool> NewBackgroundWindow;
        event GenericEventHandler<T, string, bool> NewForegroundWindow;
        event EventHandler StopClick;
        event EventHandler ReloadClick;
        event EventHandler NextClick;
        event EventHandler BackClick;
        event EventHandler HomeClick;
        event EventHandler GoClick;

        string Navigate(string url);
        string Navigate(Uri uri);
        string NavigateAgain();
        bool NavigateBack();
        bool NavigateNext();

        void Reload();

        T GoToMainFrame();
        T GoToParentFrame();
        T GoToFrame(int index);

        string Open(string arg);
        string Save(string path);
        void Print();
        }
}
