using GG40.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GG40.Support
{
    public class Timer
    {
        private DispatcherTimer _dispatcherTimer;

        public Timer(CallbackHandler callback, TimeSpan interval)
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += (sender, args) => callback?.Invoke();
            _dispatcherTimer.Interval = interval;
        }

        public void Start()
        {
            _dispatcherTimer.Start();
        }

        public void Stop() { _dispatcherTimer.Stop(); }
    }
}
