using System;
using System.ServiceProcess;

namespace WorkingTools.WinAPI
{
    public class ServiceManagerExpand : ServiceManagerLite
    {
        public virtual bool WaitForStatus(ServiceControllerStatus status, out Exception ex)
        {
            return WaitForStatus(status, null, out ex);
        }

        public virtual bool Start(TimeSpan? waitTimeSpan, out Exception ex)
        {
            Exception exception;
            if (!Start(out exception))
            {
                ex = exception;
                return false;
            }

            return WaitForStatus(ServiceControllerStatus.Running, waitTimeSpan, out ex);
        }

        public virtual bool Start(bool wait, out Exception ex)
        {
            Exception exception;
            if (!Start(out exception))
            {
                ex = exception;
                return false;
            }

            if (wait) return WaitForStatus(ServiceControllerStatus.Running, null, out ex);

            ex = null;
            return true;
        }


        public virtual bool Pause(TimeSpan? waitTimeSpan, out Exception ex)
        {
            Exception exception;
            if (!Pause(out exception))
            {
                ex = exception;
                return false;
            }

            return WaitForStatus(ServiceControllerStatus.Paused, waitTimeSpan, out ex);
        }

        public virtual bool Pause(bool wait, out Exception ex)
        {
            Exception exception;
            if (!Pause(out exception))
            {
                ex = exception;
                return false;
            }

            if (wait) return WaitForStatus(ServiceControllerStatus.Paused, null, out ex);

            ex = null;
            return true;
        }

        public virtual bool Stop(TimeSpan? waitTimeSpan, out Exception ex)
        {
            Exception exception;
            if (!Stop(out exception))
            {
                ex = exception;
                return false;
            }

            return WaitForStatus(ServiceControllerStatus.Stopped, waitTimeSpan, out ex);
        }

        public virtual bool Stop(bool wait, out Exception ex)
        {
            Exception exception;
            if (!Stop(out exception))
            {
                ex = exception;
                return false;
            }

            if (wait) return WaitForStatus(ServiceControllerStatus.Stopped, null, out ex);

            ex = null;
            return true;
        }
    }
}
