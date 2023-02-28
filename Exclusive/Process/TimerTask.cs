using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MiMFa.Exclusive.Process
{
    public class TimerTask
    {
        public bool IsRun => Watcher == null ? false : Watcher.Enabled;
        public bool IsWatching { get; protected set; } = false;
        public bool IsFreezed => Watcher == null ? false : LatestWatching < DateTime.UtcNow.AddSeconds(-FreezingTime);
        public int FreezingTime { get; protected set; } = 60 * 15;
        public DateTime StartTime { get; protected set; } = Default.SystemTime;
        public DateTime LatestWatching { get; protected set; } = Default.SystemTime;
        public int Interval { get; set; } = 1000;
        public long RunTick { get; protected set; } = 1;
        public Timer Watcher { get; protected set; } = null;

        public event ElapsedEventHandler WatchAction=null;

        public TimerTask(ElapsedEventHandler watchAction = null)
        {
            WatchAction = watchAction ?? WatchAction;
        }

        public void Start(ElapsedEventHandler watchAction = null,int? interval = null)
        {
            if (Watcher != null) try { Watcher.Stop(); } catch { }
            WatchAction = watchAction?? WatchAction;
            Watcher = new Timer();
            Watcher.Interval = Interval = interval?? Interval;
            Watcher.Elapsed += WatcherElapsed;
            IsWatching = false;
            StartTime =
            LatestWatching =Default.SystemTime;
            try { Watcher.Start(); } catch { }
        }

        public virtual void WatcherElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsWatching || Statement.Interrupt) return;
            try
            {
                IsWatching = true;
                if (WatchAction != null) WatchAction(this,e);
                RunTick++;
                LatestWatching = Default.SystemTime;
            }
            finally { IsWatching = false; }
        }

        public void Pause()
        {
            if (Watcher != null) try { Watcher.Stop(); } catch { }
        }
        public void Resume()
        {
            if (Watcher != null) try { Watcher.Start(); } catch { }
        }
        public void Stop()
        {
            if (Watcher != null) try { Watcher.Stop(); } catch { }
        }
    }
}
