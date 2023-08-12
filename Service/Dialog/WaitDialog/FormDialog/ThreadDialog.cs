using MiMFa.Exclusive.Animate;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiMFa.Engine.Translate;
using MiMFa.Engine.Template;

namespace MiMFa.Service.Dialog.WaitDialog.FormDialog
{
    public partial class ThreadDialog : Form, ITranslatable, ITemplatable
    {

        public event RunWorkerCompletedEventHandler Finished = (s, e) => { };
        public void OnFinish(RunWorkerCompletedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Finished(this, e);
            Cancel();
        }

        public bool TryCatchBlock = true;
        public bool ShowMessages = false;
        public bool SilentMode = false;


        private static FormMove FM;
        public ThreadDialog(bool tryCatchBlock = true, bool showMessages = true)
        {
            InitializeComponent();
            TryCatchBlock = tryCatchBlock;
            ShowMessages = showMessages;
            FM = new FormMove(this,this, TitleLabel,panel1, TextLabel, WatingBar);
            Template(this);
        }


        /// <summary>
        /// Start Process in a Silent safe Thread
        /// </summary>
        /// <param name="action">Main Action</param>
        /// <param name="threadMode">
        /// case 0: StartThread(action); 
        /// case 1: StartBackground;
        /// case 2: StartForeground; 
        /// case 3: StartTask; 
        /// case 4: StartBackgroundWorker; 
        /// default: StartThread; 
        /// </param>
        public virtual void Silent(string text, DoWorkEventHandler action, int threadMode = -1)
        {
            if (text != null) Text = TextLabel.Text = Translate(text);
            SilentMode = true;
            Start(action, threadMode);
        }
        /// <summary>
        /// Start Process with a safe Dialogs Thread
        /// </summary>
        /// <param name="action">Main Action</param>
        /// <param name="threadMode">
        /// case 0: StartThread(action); 
        /// case 1: StartBackground;
        /// case 2: StartForeground; 
        /// case 3: StartTask; 
        /// case 4: StartBackgroundWorker; 
        /// default: StartThread; 
        /// </param>
        public virtual void Show(string text, DoWorkEventHandler action, int threadMode = -1)
        {
            if (text != null) Text = TextLabel.Text = Translate(text);
            Start(action, threadMode);
            if (!IsDisposed) try { Show(); } catch { }
        }
        /// <summary>
        /// Start Process in a safe Waiting Dialogs Thread
        /// </summary>
        /// <param name="action">Main Action</param>
        /// <param name="threadMode">
        /// case 0: StartThread(action); 
        /// case 1: StartBackground;
        /// case 2: StartForeground; 
        /// case 3: StartTask; 
        /// case 4: StartBackgroundWorker; 
        /// default: StartThread; 
        /// </param>
        public virtual DialogResult ShowDialog(string text, DoWorkEventHandler action,int threadMode = -1)
        {
            if (text != null) Text = TextLabel.Text = Translate(text);
            Start(action, threadMode);
            if (!IsDisposed) try { return ShowDialog(); } catch { }
            return DialogResult;
        }


        /// <summary>
        /// Start Process in a safe Thread
        /// </summary>
        /// <param name="action">Main Action</param>
        /// <param name="threadMode">
        /// case 0: StartThread(action); 
        /// case 1: StartBackground;
        /// case 2: StartForeground; 
        /// case 3: StartTask; 
        /// case 4: StartBackgroundWorker; 
        /// default: StartThread; 
        /// </param>
        public virtual void Start(DoWorkEventHandler action, int threadMode = -1)
        {
            switch (threadMode)
            {
                case 0:
                    StartThread(action);
                    break;
                case 1:
                    StartBackground(action);
                    break;
                case 2:
                    StartForeground(action);
                    break;
                case 3:
                    StartTask(action);
                    break;
                case 4:
                    StartBackgroundWorker(action);
                    break;
                default:
                    StartThread(action);
                    break;
            }
        }

        public Thread WorkThread = null;
        public virtual void StartThread(DoWorkEventHandler action, bool background = true, ApartmentState apartmentState = ApartmentState.STA)
        {
            if (WorkThread != null && WorkThread.IsAlive) try { WorkThread.Abort(); } catch { }
            ProcessService.ReduceMemory(false);
            if (TryCatchBlock)
                WorkThread = new Thread((a) =>
                {
                    try
                    {
                        action(this, new DoWorkEventArgs(this));
                    }
                    catch (Exception ex) { if (ShowMessages) DialogService.ShowMessage(ex); }
                    OnFinish(null);
                });
            else WorkThread = new Thread((a) =>
            {
                action(this, null);
                OnFinish(null);
            });
            WorkThread.IsBackground = background;
            WorkThread.SetApartmentState(apartmentState);
            if (SilentMode) WorkThread.Start();
            else Load += (s, e) => WorkThread.Start();
        }
        public virtual void StartBackground(DoWorkEventHandler action)
        {
            StartThread(action, true);
        }
        public virtual void StartForeground(DoWorkEventHandler action)
        {
            StartThread(action, false);
        }


        public Task WorkTask = null;
        public virtual void StartTask(DoWorkEventHandler action)
        {
            if (WorkTask != null && !WorkTask.IsCompleted) try { WorkTask.Dispose(); } catch { }
            ProcessService.ReduceMemory(false);

            if (TryCatchBlock)
                WorkTask = new Task(() =>
                {
                    try
                    {
                        action(this, new DoWorkEventArgs(this));
                    }
                    catch (Exception ex) { if (ShowMessages) DialogService.ShowMessage(ex); }
                    OnFinish(null);
                });
            else
                WorkTask = new Task(() =>
                {
                        action(this, new DoWorkEventArgs(this));
                    OnFinish(null);
                });
            if (SilentMode) WorkTask.Start();
            else Load += (s, e) => WorkTask.Start();
        }

        public BackgroundWorker WorkBackground = null;
        public virtual void StartBackgroundWorker(DoWorkEventHandler action)
        {
            if (WorkBackground != null && WorkBackground.IsBusy) try { WorkBackground.Dispose(); } catch { }
            ProcessService.ReduceMemory(false);

            WorkBackground = new BackgroundWorker();
            WorkBackground.WorkerSupportsCancellation = true;
            if (TryCatchBlock)
                WorkBackground.DoWork += (s, e) =>
                {
                    try { action(s, e); } 
                    catch (Exception ex) { if (ShowMessages) DialogService.ShowMessage(ex); }
                };
            else WorkBackground.DoWork += action;
            WorkBackground.RunWorkerCompleted += (s, e) =>
            {
                OnFinish(e);
            };

            if (SilentMode) WorkBackground.RunWorkerAsync();
            else Load += (s, e) => WorkBackground.RunWorkerAsync();
        }

        public virtual bool Cancel()
        {
            if (!SilentMode && !IsDisposed)
                try { ControlService.SetControlThreadSafe(this, (args) => Close()); return true; } catch { try { Close(); return true; } catch { } }
            else return Abort();
            return false;
        }
        public virtual bool Abort()
        {
            bool b = false;
            if (WorkThread != null && WorkThread.IsAlive) try { WorkThread.Abort(); b = true/*!WorkThread.IsAlive*/; } catch { }
            if (WorkTask != null && !WorkTask.IsCompleted) try { /*WorkTask.Wait();*/ WorkTask.Dispose(); b = true/*(WorkTask.IsCanceled || WorkTask.IsFaulted || WorkTask.IsCompleted)*/; } catch { }
            if (WorkBackground != null && WorkBackground.IsBusy) try { WorkBackground.CancelAsync(); WorkBackground.Dispose(); b = true/*!WorkBackgroundWorker.IsBusy*/; } catch { }
            ProcessService.ReduceMemory(false);
            return b;
        }

        private void ThreadDialog_Shown(object sender, EventArgs e)
        {
            SilentMode = false;
        }
        private void WaitDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
           //e.Cancel = !Abort();
        }
        private void ThreadDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Abort();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public virtual string Translate(string message)
        {
            return Default.Translate(message);
        }
        public virtual void Template(Control control)
        {
            Default.Template(control);
        }
    }
}
