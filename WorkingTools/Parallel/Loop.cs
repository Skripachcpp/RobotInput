using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkingTools.Parallel
{
    /// <summary>
    /// Вызов делегата с заданным интервалом
    /// </summary>
    /// <remarks>отсчет веремени до повторного вызова ведется с момента окончания работы делегата</remarks>
    public class Loop : IDisposable
    {
        protected readonly object Lock = new object();

        private readonly int _interval;//интервал вызова делегата

        protected readonly ManualResetEvent EventStop = new ManualResetEvent(true);//событие завершения работы

        protected CancellationTokenSource SoftStop;//токен остановки

        protected readonly Action Callback;//исполняемый делегат

        protected bool Started { set; get; }


        private Loop(int interval)
        {
            _interval = interval < 0 ? 0 : interval;

            Started = false;
        }

        public Loop(Action сallback, int interval)
            : this(interval)
        {
            Callback = сallback;
        }


        public int Interval { get { return _interval; } }


        private class BeginArgs
        {
            public BeginArgs(int firstStart, CancellationToken softStopToken)
            {
                if (softStopToken == null) throw new ArgumentNullException("softStopToken");

                FirstStart = firstStart < 0 ? 0 : firstStart;
                SoftStopToken = softStopToken;
            }

            public int FirstStart { get; private set; }

            public CancellationToken SoftStopToken { get; private set; }
        }

        protected virtual void Begin(object beginArgs)
        {
            if (!(beginArgs is BeginArgs))
                throw new ArgumentOutOfRangeException("beginArgs", string.Format("ожидается {0}", typeof(BeginArgs)));

            var args = (BeginArgs)beginArgs;

            var token = args.SoftStopToken;

            if (!token.IsCancellationRequested && !token.WaitHandle.WaitOne(args.FirstStart))
            {
                Callback.Invoke();
                while (!token.IsCancellationRequested && !token.WaitHandle.WaitOne(Interval))
                    Callback.Invoke();
            }
        }

        protected virtual void End()
        {
            lock (Lock)
            {
                Started = false;
            }

            EventStop.Set();
        }

        protected virtual void End(Task task)
        {
            End();
            task.Dispose();
        }

        public virtual void Start(int firstStart = 0, bool async = true)
        {
            CancellationTokenSource softStop;
            lock (Lock)//не запускать если уже запущен
            {
                if (Started) return;
                else Started = true;


                if (SoftStop == null || SoftStop.IsCancellationRequested)
                {
                    if (SoftStop != null)
                        SoftStop.Dispose();

                    SoftStop = new CancellationTokenSource();
                }

                softStop = SoftStop;
            }

            EventStop.Reset();

            var args = new BeginArgs(firstStart, softStop.Token);
            if (async)
                Task.Factory.StartNew(Begin, args).ContinueWith(End);
            else
            {
                Begin(args);
                End();
            }
        }

        public virtual void Stop(bool wait = false)
        {
            CancellationTokenSource softStop;
            lock (Lock)
            {
                if (!Started) return;
                softStop = SoftStop;
            }

            if (softStop != null)
                softStop.Cancel();

            if (wait) Wait();
        }

        public void Wait()
        { EventStop.WaitOne(); }

        /// <summary>
        /// Освободить ресурсы используемые объектом
        /// </summary>
        /// <param name="wait">дождаться окончания освобождения ресурсов объектом</param>
        public void Dispose(bool wait)
        {
            Stop(wait);
            if (SoftStop != null) SoftStop.Dispose();
        }

        public void Dispose()
        { Dispose(true); }
    }
}