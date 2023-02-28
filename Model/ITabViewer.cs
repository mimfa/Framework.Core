using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    public interface ITabViewer<T> 
    {
        T NewTab();
        T GoToTab(int index);
        T GoToTab(string windowName);
        T GoToFirstTab();
        T GoToLastTab();

        T CloseTab();
        T CloseTab(int index);
        T CloseTab(string windowName);
        /// <summary>
        /// Close All Tabs
        /// </summary>
        /// <returns></returns>
        T CloseAllTabs();
    }
}
