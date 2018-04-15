using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using WorkingTools.Parallel;

namespace WorkingTools.WinAPI
{
    public abstract class ServiceAbstract : ServiceAbstractLite
    {
        private const int AutoRefreshInfoIntervalMin = 500;
        private const int AutoRefreshInfoIntervalDefault = 2500;
        private readonly Loop _loop;

        private object _oldInfo;

        protected ServiceAbstract(int? autoRefreshInfoInterval = null)
        {
            var interval = autoRefreshInfoInterval ?? AutoRefreshInfoIntervalDefault;
            if (interval < AutoRefreshInfoIntervalMin) interval = AutoRefreshInfoIntervalMin;

            _loop = new Loop(() =>
            {
                if (InfoChange == null) return;

                var newInfo = CreateInfo();
                if (newInfo != _oldInfo)
                {
                    _oldInfo = newInfo;
                    OnInfoChange((ServiceInfo)newInfo);
                }
            }, interval);
            _loop.Start();
        }

        public virtual void Pause() { Exception exception; if (!Pause(out exception) && exception != null) throw exception; }
        public virtual void Stop() { Exception exception; if (!Stop(out exception) && exception != null) throw exception; }
        public virtual void Start() { Exception exception; if (!Start(out exception) && exception != null) throw exception; }
        public virtual void WaitForStatus(ServiceControllerStatus status, TimeSpan? timeSpan) { Exception exception; if (!WaitForStatus(status, timeSpan, out exception) && exception != null) throw exception; }

        public virtual void Install()
        {
            Exception exception;
            if (!Install(out exception) && exception != null)
                throw exception;
        }

        public virtual void Install(Action ifInstalled, Action<Exception> ifNotInstalled = null)
        {
            Task.Factory.StartNew(() =>
            {
                Exception exception;
                if (Install(out exception))
                {
                    if (ifInstalled != null)
                        ifInstalled();
                }
                else
                {
                    if (ifNotInstalled != null)
                        ifNotInstalled(exception);
                }
            });
        }

        public virtual void Uninstall()
        {
            Exception exception;
            if (!Uninstall(out exception) && exception != null)
                throw exception;
        }

        public virtual void Uninstall(Action ifUninstalled, Action<Exception> ifNotUninstalled = null)
        {
            Task.Factory.StartNew(() =>
            {
                Exception exception;
                if (Uninstall(out exception))
                {
                    if (ifUninstalled != null)
                        ifUninstalled();
                }
                else
                {
                    if (ifNotUninstalled != null)
                        ifNotUninstalled(exception);
                }
            });
        }

        protected virtual object CreateInfo()
        {
            var info = new ServiceInfo();
            Info(info);

            return info;
        }

        public ServiceInfo Info(ServiceInfo serviceInfo = null)
        {
            if (serviceInfo == null) serviceInfo = (ServiceInfo)CreateInfo();
            base.Info(serviceInfo);

            return serviceInfo;
        }

        public new class ServiceInfo : ServiceAbstractLite.ServiceInfo
        {
        }

        public event EventHandler<ServiceInfo> InfoChange;

        protected virtual void OnInfoChange(ServiceInfo e)
        {
            var handler = InfoChange;
            if (handler != null) handler(this, e);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_loop != null) _loop.Dispose();
        }
    }
}