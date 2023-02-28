using MiMFa.General;
using MiMFa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Engine.Web
{
    public interface ITabWebBrowser<T> : ITabViewer<T>
    {
        event EventHandler Initialized;
        event GenericEventListener<T, string> Navigating;
        event GenericEventListener<T, string> Navigated;
        event GenericEventListener<T, string, int, object> DocumentCompleted;
        event GenericEventHandler<T, string, object, string> BeginDownload;
        event GenericEventHandler<T, string, int, string, object, bool> Downloading;
        event GenericEventListener<T, string, bool, string, object> FinishDownload;
        event GenericEventHandler<T, string, bool> TabCreated;
        event GenericEventHandler<T, string, bool> NewBackgroundTab;
        event GenericEventHandler<T, string, bool> NewForegroundTab;
        event EventHandler StopClick;
        event EventHandler ReloadClick;
        event EventHandler NextClick;
        event EventHandler BackClick;
        event EventHandler HomeClick;
        event EventHandler GoClick;

        T Navigate(string url);
        T Navigate(Uri uri);
        T NavigateAgain();
        T NavigateBack();
        T NavigateNext();

        T Reload();

        T GoToMainFrame();
        T GoToParentFrame();
        T GoToFrame(int index);

        T Open(string arg);
        T Save(string path);
        T Print();
    }
}
