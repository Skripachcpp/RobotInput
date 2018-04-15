using System;
using System.ServiceProcess;

namespace WorkingTools.WinAPI
{
    /// <summary>
    /// јбстрактный класс управлени€ какой то конкретной службой
    /// </summary>
    public abstract class ServiceAbstractLite : IDisposable
    {
        protected const string ExNotRightsMessage = "не достаточно прав дл€ выполнени дестви€";
        protected readonly ServiceManager ServiceManager;

        private readonly bool _isAdmin;

        protected ServiceAbstractLite()
        {
            _isAdmin = Acces.IsAdmin();
            ServiceManager = new ServiceManager();
        }

        public string ServiceName { get; private set; }

        public void Attach(string serviceName)
        {
            ServiceName = serviceName;

            Exception exception;
            ServiceManager.Attach(serviceName, out exception);
        }

        public class RightsRequiredArgs : EventArgs
        {
            public bool Cancel { get; set; }
        }

        public event EventHandler<RightsRequiredArgs> RightsRequired;
        protected virtual void OnRightsRequired(RightsRequiredArgs e) { var handler = RightsRequired; if (handler != null) handler(this, e); }

        /// <summary>
        /// ѕроверка и запрос прав на выполнение действи€
        /// </summary>
        protected virtual bool ChangeRights(out Exception ex)
        {
            //TODO: данна€ реализаци€ €вл€етс€ костылем и может быть переопределена в наследнике
            if (!_isAdmin)
            {
                var args = new RightsRequiredArgs();
                OnRightsRequired(args);
                if (args.Cancel)
                {
                    ex = null;
                    return false;
                }
            }

            ex = null;
            return true;
        }

        public virtual bool Pause(out Exception ex)
        {
            if (!ChangeRights(out ex)) return false;
            return ServiceManager.Pause(out ex);
        }

        public virtual bool Stop(out Exception ex)
        {
            if (!ChangeRights(out ex)) return false;
            return ServiceManager.Stop(out ex);
        }

        public virtual bool Start(out Exception ex)
        {
            if (!ChangeRights(out ex)) return false;
            return ServiceManager.Start(out ex);
        }

        public virtual bool WaitForStatus(ServiceControllerStatus status, TimeSpan? timeSpan, out Exception ex) { return ServiceManager.WaitForStatus(status, timeSpan, out ex); }

        public virtual bool Install(out Exception ex)
        {
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                ex = new Exception("им€ службы пусто или отсутствует");
                return false;
            }

            if (ServiceManager.ServiceName == ServiceName)
            {
                ex = new Exception("служба уже установлена");
                return false;
            }

            Exception exception;
            if (!ChangeRights(out exception))
            {
                ex = exception;
                return false;
            }

            try
            {
                Install(ServiceName);
            }
            catch (Exception except)
            {
                ex = except;
                return false;
            }

            return ServiceManager.Attach(ServiceName, out ex);
        }

        public virtual bool Uninstall(out Exception ex)
        {
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                ex = new Exception("им€ службы пусто или отсутствует");
                return false;
            }

            if (ServiceManager.ServiceName != ServiceName)
            {
                ex = new Exception("служба не установлена");
                return false;
            }

            Exception exception;
            if (!ChangeRights(out exception))
            {
                ex = exception;
                return false;
            }

            try
            {
                Uninstall(ServiceName);
                ServiceManager.Detach();
            }
            catch (Exception except)
            {
                ex = except;
                return false;
            }

            ex = null;
            return true;
        }

        public ServiceInfo Info(ServiceInfo serviceInfo = null)
        {
            if (serviceInfo == null) serviceInfo = new ServiceInfo();
            ServiceManager.Info(serviceInfo);

            serviceInfo.Installed = serviceInfo.ServiceName == ServiceName;

            serviceInfo.ServiceName = ServiceName;

            return serviceInfo;
        }

        public class ServiceInfo : ServiceManager.ServiceInfo
        {
            public bool Installed { get; set; }

            #region Equals

            protected bool Equals(ServiceInfo other)
            {
                return base.Equals(other) && Installed.Equals(other.Installed);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (base.GetHashCode() * 397) ^ Installed.GetHashCode();
                }
            }

            public static bool operator ==(ServiceInfo left, ServiceInfo right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ServiceInfo left, ServiceInfo right)
            {
                return !Equals(left, right);
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            #endregion Equals
        }

        public virtual void Dispose()
        {
            if (ServiceManager != null) ServiceManager.Dispose();
        }


        protected abstract void Install(string serviceName);

        protected abstract void Uninstall(string serviceName);
    }
}