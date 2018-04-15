using System;
using System.ServiceProcess;

namespace WorkingTools.WinAPI
{
    public sealed class ServiceManager : ServiceManagerExpand
    {
        public ServiceManager()
        {
        }

        public void WaitForStatus(ServiceControllerStatus status)
        {
            Exception exception;
            if (!WaitForStatus(status, out exception) && exception != null)
                throw exception;
        }

        /// <summary>
        /// Подключиться к службе
        /// </summary>
        /// <param name="service"></param>
        public void Attach(ServiceController service)
        {
            Exception exception;
            if (!Attach(service, out exception) && exception != null)
                throw exception;
        }

        /// <summary>
        /// Подключиться к службе
        /// </summary>
        /// <param name="serviceName"></param>
        public void Attach(string serviceName)
        {
            Exception exception;
            if (!Attach(serviceName, out exception) && exception != null)
                throw exception;
        }

        public ServiceControllerStatus? GetStatus()
        {
            Exception exception;
            var st = GetStatus(out exception);
            if (exception != null)
                throw exception;

            return st;
        }


        public void Start(TimeSpan? waitTimeSpan)
        {
            Exception exception;
            if (!Start(waitTimeSpan, out exception) && exception != null)
                throw exception;
        }

        public void Start(bool wait = false)
        {
            Exception exception;
            if (!Start(wait, out exception) && exception != null)
                throw exception;
        }


        public void Pause(TimeSpan? waitTimeSpan)
        {
            Exception exception;
            if (!Pause(waitTimeSpan, out exception) && exception != null)
                throw exception;
        }

        public void Pause(bool wait = false)
        {
            Exception exception;
            if (!Pause(wait, out exception) && exception != null)
                throw exception;
        }


        public void Stop(TimeSpan? waitTimeSpan)
        {
            Exception exception;
            if (!Stop(waitTimeSpan, out exception) && exception != null)
                throw exception;
        }

        public void Stop(bool wait = false)
        {
            Exception exception;
            if (!Stop(wait, out exception) && exception != null)
                throw exception;
        }

        public void ExecuteCommand(int command)
        {
            Exception exception;
            if (!ExecuteCommand(command, out exception) && exception != null)
                throw exception;
        }

        public ServiceInfo Info(ServiceInfo serviceInfo = null)
        {
            if (serviceInfo == null) serviceInfo = new ServiceInfo();

            Exception exception;
            //если serviceController.Refresh()
            //не был вызван до обрщения к публичным свойствам службы,
            //то их значение может быть не актуальным
            var status = GetStatus(out exception);

            serviceInfo.CanStart = HowStart(status, out exception) != HowStartType.NoWay;
            serviceInfo.CanPause = CanPause(status, out exception);
            serviceInfo.CanStop = CanStop(status, out exception);
            serviceInfo.Status = status;


            serviceInfo.ServiceName = ServiceName;
            serviceInfo.MachineName = MachineName;
            serviceInfo.DisplayName = DisplayName;

            return serviceInfo;
        }

        public class ServiceInfo : EventArgs
        {
            public string ServiceName { get; set; }
            public string MachineName { get; set; }
            public string DisplayName { get; set; }

            public ServiceControllerStatus? Status { get; set; }

            public bool CanStart { get; set; }
            public bool CanPause { get; set; }
            public bool CanStop { get; set; }


            #region Equals

            protected bool Equals(ServiceInfo other)
            {
                return string.Equals(ServiceName, other.ServiceName) && string.Equals(MachineName, other.MachineName) && string.Equals(DisplayName, other.DisplayName) && Status == other.Status && CanStart.Equals(other.CanStart) && CanPause.Equals(other.CanPause) && CanStop.Equals(other.CanStop);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (ServiceName != null ? ServiceName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (MachineName != null ? MachineName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Status.GetHashCode();
                    hashCode = (hashCode * 397) ^ CanStart.GetHashCode();
                    hashCode = (hashCode * 397) ^ CanPause.GetHashCode();
                    hashCode = (hashCode * 397) ^ CanStop.GetHashCode();
                    return hashCode;
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
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ServiceInfo)obj);
            }

            #endregion Equals
        }

    }
}
