using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using MiMFa.Service.Dialog.WaitDialog.FormDialog;
using System.Management;
using Timer = System.Threading.Timer;

namespace MiMFa.Service
{
    public class ProcessService
    {

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        public static void OpenWithPicturePreview(Image image, string tempAddress)
        {
            if (File.Exists(tempAddress)) File.Delete(tempAddress);
            image.Save(tempAddress);
            Process.Start(tempAddress);
        }
        public static void Run(string address)
        {
            try { Process.Start(address); } catch { }
        }
        public static void Run(string filename,string address)
        {
            try { Process.Start(filename,address); } catch { }
        }
        public static void RunTopMost(string address)
        {
            if (File.Exists(address))
            {
                Process p = new Process();
                p.StartInfo.FileName =address;
                p.Start();
                SetTopMost(p);
            }
        }
        public static void SetTopMost(Process p)
        {
            SetWindowPos(p.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }


        public static void Show(string message, string title=null)
        {
              Intermediate.NotepadHelper.ShowMessage(message,title);
        }
        public static bool ReduceMemory(bool byProcessWorkingSetSize = false)
        {
            bool b = true;
            if (byProcessWorkingSetSize)
            {
                IntPtr pHandle = GetCurrentProcess();
                b = SetProcessWorkingSetSize(pHandle, -1, -1);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            return b;
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetCurrentProcess();

        public static void Run(object mainAction)
        {
            throw new NotImplementedException();
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr point);
        public static int SetForegroundWindow(Process process) { try { return SetForegroundWindow(process.Handle); } catch { return 0; } }

        public static int SetBackgroundWindow(IntPtr point) { SendKeys.SendWait("%{TAB}"); return 0; }
        public static int SetBackgroundWindow(Process process) { SendKeys.SendWait("%{TAB}"); return 0; }

        public static bool IsWindow(Process process)
        {
            return !IsService(process);
        }
        public static bool IsWindow(int id) => IsWindow(FindProcess(id));
        public static bool IsWindow(string name) => IsWindow(FindProcess(name));
        public static bool IsService(Process process) => IsService(process.Id);
        public static bool IsService(int id)
        {
            using (ManagementObjectSearcher Searcher = new ManagementObjectSearcher(
            "SELECT * FROM Win32_Service WHERE ProcessId =" + "\"" + id + "\""))
            {
                foreach (ManagementObject service in Searcher.Get())
                    return true;
            }
            return false;
        }
        public static bool IsService(string name)
        {
            using (ManagementObjectSearcher Searcher = new ManagementObjectSearcher(
            "SELECT * FROM Win32_Service WHERE Name =" + "\"" + name + "\""))
            {
                foreach (ManagementObject service in Searcher.Get())
                    return true;
            }
            return false;
        }


        public static Process[] GetProcesses(string name) => Process.GetProcessesByName(name);
        public static Process GetProcess(int id) => Process.GetProcessById(id);
        public static Process[] GetSubProcesses(Process process) => FindProcesses(p => GetSuperProcessID(p.Id) == process.Id).ToArray();
        public static Process GetSuperProcess(Process process) => FindProcess(GetSuperProcessID(process.Id));
        public static Process GetSuperProcess(int id) => FindProcess(GetSuperProcessID(id));
        public static int GetSuperProcessID(int id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject($"win32_process.handle='{id}'"))
                try
                {
                    mo.Get();
                    parentPid = Convert.ToInt32(mo["ParentProcessId"]);
                }
                catch { }
            return parentPid;
        }

        public static Process FindProcess(IntPtr handle) => FindProcess(p => p.Handle == handle);
        public static Process FindProcess(string title) => FindProcess(p=> p.MainWindowTitle == title);
        public static Process FindProcess(int id) => FindProcess(p=> p.Id == id);
        public static Process FindProcess(Func<Process,bool> comparer)
        {
            foreach (Process p in Process.GetProcesses().Reverse())
                if (comparer(p))
                    return p;
            return null;
        }
        public static IEnumerable<Process> FindProcesses(Func<Process, bool> comparer)
        {
            foreach (Process p in Process.GetProcesses().Reverse())
                if (comparer(p))
                    yield return p;
            yield break;
        }

        public static void WaitUntilLoad(Process process, int? millisecond = null)
        {
            while ((!millisecond.HasValue || millisecond > 0) && string.IsNullOrEmpty(process.MainWindowTitle))
            {
                Thread.Sleep(100);
                if (millisecond.HasValue) millisecond -= 100;
                process.Refresh();
            }
        }
        public static void WaitUntilExit(Process process, int? millisecond = null)
        {
            if (millisecond.HasValue) process.WaitForExit(millisecond.Value);
            else process.WaitForExit();
        }

        public static Process StartCommand(params string[] commands) => StartCommand(commands, false);
        public static Process StartCommand(IEnumerable<string> commands, bool inBackground, bool runAsAdministrator = true)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            if(commands.Any()) p.StartInfo.Arguments = @"/C " + string.Join("&&", commands);
            if (runAsAdministrator)
                p.StartInfo.Verb = "runas";
            if (inBackground)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            p.Start();
            return p;
        }

        public static Dictionary<long,Timer> ProcessTimers = new Dictionary<long, Timer>();
        public static void Run<TOutput>(int milisecondDelay, Func<TOutput> action, bool background = true)
        {
            var l = DateTime.UtcNow.Ticks;
            ProcessTimers.Add(l, new Timer((s) =>
            {
                try
                {
                    var ll = (long)s;
                    Run(action, background);
                    ProcessTimers[ll].Dispose();
                    ProcessTimers.Remove(ll);
                }
                catch { }
            }, l, milisecondDelay, milisecondDelay));
        }
        public static void Run(int milisecondDelay, Action action, bool background = true)
        {
            var l = DateTime.UtcNow.Ticks;
            ProcessTimers.Add(l, new Timer((s) =>
            {
                try
                {
                    var ll = (long)s;
                    Run(action, background);
                    ProcessTimers[ll].Dispose();
                    ProcessTimers.Remove(ll);
                }
                catch { }
            }, l, milisecondDelay, milisecondDelay));
        }
        public static Thread Run<TOutput>(Func<TOutput> action, bool background = true)
        {
            var th = new Thread(() => action());
            th.IsBackground = background;
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            return th;
        }
        public static Thread Run(Action action, bool background = true)
        {
            return RunThread(new ThreadStart(action), background);
        }
        public static Form RunDialog(string message, DoWorkEventHandler work, RunWorkerCompletedEventHandler finish = null)
        {
            using (ThreadDialog wd = new ThreadDialog(true, true))
            {
                if (finish != null) wd.Finished += finish;
                wd.ShowDialog(message, work,3);
                return wd;
            }
        }
        public static Form RunScreen(string message, DoWorkEventHandler work, RunWorkerCompletedEventHandler finish = null)
        {
            ThreadDialog wd = new ThreadDialog(true, true);
            if (finish != null) wd.Finished += finish;
            wd.Show(message, work,3);
            return wd;
        }
        public static Thread RunThread(ThreadStart action, bool background = true)
        {
            var th = new Thread(action);
            th.IsBackground = background;
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            return th;
        }
        public static Task RunTask(Action action, TaskCreationOptions taskCreationOptions = TaskCreationOptions.LongRunning)
        {
            var t = new Task(action, taskCreationOptions);
            t.RunSynchronously();
            return t;
        }
        public static Task RunTask<InputT>(Action<InputT> task, InputT arg = default(InputT), TaskCreationOptions taskCreationOptions = TaskCreationOptions.LongRunning)
        {
            var t = new Task(() => task(arg), taskCreationOptions);
            t.RunSynchronously();
            return t;
        }
        public static Task<OutputT> RunTask<InputT, OutputT>(Func<InputT, OutputT> task, InputT arg = default(InputT))
        {
            return Task.Run(() => task(arg));
        }
        public static OutputT TaskHandler<OutputT>(Task<OutputT> task, int secondsLimit = 2)
        {
            try
            {
                task.Wait(new TimeSpan(0, 0, secondsLimit));
                return task.Result;
            }
            catch { return default(OutputT); }
        }
        public static OutputT TaskHandler<OutputT>(Task<OutputT> task, int secondsLimit, OutputT defaultVal)
        {
            return TaskHandler(task, secondsLimit);
        }
        public static OutputT TaskHandler<InputT, OutputT>(Task<InputT> task, int secondsLimit, InputT defaultVal, Func<InputT, OutputT> convertor)
        {
            return convertor(TaskHandler(task, secondsLimit, defaultVal));
        }


        public static void Wait(int miliseconds = 1000)
        {
            Task.Delay(miliseconds).Wait();
        }

    }
}
