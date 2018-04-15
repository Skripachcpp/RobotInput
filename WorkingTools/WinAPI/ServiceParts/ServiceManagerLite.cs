using System;
using System.Linq;
using System.ServiceProcess;

namespace WorkingTools.WinAPI
{
    public class ServiceManagerLite : IDisposable
    {
        private const string ExNotAttachMessage = "отсутствует подключение к слежбе";

        private ServiceController _serviceController;

        public string MachineName
        {
            get
            {
                string machineName = null;
                var serviceController = _serviceController;
                try { machineName = serviceController != null ? serviceController.MachineName : null; }
                catch (Exception) { }
                return machineName;
            }
        }

        public string ServiceName
        {
            get
            {
                string serviceName = null;
                var serviceController = _serviceController;
                try { serviceName = serviceController != null ? serviceController.ServiceName : null; }
                catch (Exception) { }
                return serviceName;
            }
        }

        public string DisplayName
        {
            get
            {
                string displayName = null;
                var serviceController = _serviceController;
                try { displayName = serviceController != null ? serviceController.DisplayName : null; }
                catch (Exception) { }
                return displayName;
            }
        }

        /// <summary>
        /// Подключиться к службе
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool Attach(ServiceController service, out Exception ex)
        {
            if (service == null)
            {
                ex = new ArgumentOutOfRangeException("service", service, "отсутствует контролер службы");
                return false;
            }

            ex = null;
            _serviceController = service;

            return true;
        }

        /// <summary>
        /// Подключиться к службе
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool Attach(string serviceName, out Exception ex)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                ex = new ArgumentOutOfRangeException("serviceName", "имя службы пусто или отсутствует");
                return false;
            }

            var serviceController = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == serviceName);

            if (serviceController == null)
            {
                ex = new ArgumentOutOfRangeException("serviceName", serviceName, "служба с указаным именем не была найдена");
                return false;
            }

            return Attach(serviceController, out ex);
        }

        /// <summary>
        /// Отключиться от службы
        /// </summary>
        public void Detach()
        {
            var serviceController = _serviceController;
            if (serviceController != null)
            {
                try
                {
                    serviceController.Close();
                    serviceController.Dispose();
                }
                catch (Exception)
                {

                }
            }

            _serviceController = null;
        }

        /// <summary>
        /// Получить статус службы
        /// </summary>
        /// <returns>статус состояния службы или null, в случае к службе обратиться не удалось</returns>
        public virtual ServiceControllerStatus? GetStatus(out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return null; }

            try
            {
                serviceController.Refresh();
                //иногда не возможно получить статус службы, 
                //поскольку служба была удалена после подключения к ней,
                //или по другим причинам;
                //в этом случае вылетаем в исключение
                var status = serviceController.Status;
                ex = null;
                return status;
            }
            catch (Exception exception)
            {
                ex = exception;
                Detach();
                return null;
            }
        }

        /// <summary>
        /// Ожидать смены статцса
        /// </summary>
        /// <param name="status">статус</param>
        /// <param name="timeSpan">максимальное время ожидания</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual bool WaitForStatus(ServiceControllerStatus status, TimeSpan? timeSpan, out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null)
            {
                ex = new Exception(ExNotAttachMessage);
                return false;
            }

            try
            {
                if (timeSpan == null) serviceController.WaitForStatus(status);
                else serviceController.WaitForStatus(status, (TimeSpan)timeSpan);
            }
            catch (Exception exception)
            {
                ex = exception;
                return false;
            }

            ex = null;
            return true;
        }

        protected virtual bool CanPause(ServiceControllerStatus? status, out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return false; }

            if (status == ServiceControllerStatus.Running)
            {
                try
                {
                    if (!serviceController.CanPauseAndContinue)
                    {
                        ex = new Exception("служба не может быть преостановлена поскольку данная возможность не поддерживается службой");
                        return false;
                    }
                }
                catch (Exception except)
                {
                    ex = except;
                    return false;
                }

                ex = null;
                return true;
            }

            ex = new Exception(string.Format("служба не может приостановить свою работу, поскольку находится в состоянии {0}", status));
            return false;
        }

        protected virtual bool CanPause(out Exception ex)
        {
            var status = GetStatus(out ex);
            if (status == null) return false;

            return CanPause(status, out ex);
        }

        public virtual bool Pause(out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return false; }

            Exception exception;
            if (!CanPause(out exception))
            {
                ex = exception;
                return false;
            }

            try
            {
                serviceController.Pause();
            }
            catch (Exception except)
            {
                ex = except;
                return false;
            }

            ex = null;
            return true;
        }

        protected virtual bool CanStop(ServiceControllerStatus? status, out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return false; }

            if (status == ServiceControllerStatus.Running)
            {
                try
                {
                    if (!serviceController.CanStop)
                    {
                        ex = new Exception("служба не может быть оснановлена поскольку данная возможность не поддерживается службой");
                        return false;
                    }
                }
                catch (Exception except)
                {
                    ex = except;
                    return false;
                }

                ex = null;
                return true;
            }

            ex = new Exception(string.Format("служба не может остановиться, поскольку находится в состоянии {0}", status));
            return false;
        }

        protected virtual bool CanStop(out Exception ex)
        {
            var status = GetStatus(out ex);
            if (status == null) return false;

            return CanStop(status, out ex);
        }

        public virtual bool Stop(out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return false; }

            Exception exception;
            if (!CanStop(out exception))
            {
                ex = exception;
                return false;
            }

            try
            {
                serviceController.Stop();
            }
            catch (Exception except)
            {
                ex = except;
                return false;
            }

            ex = null;
            return true;
        }

        protected enum HowStartType { NoWay, Start, Continue }

        protected HowStartType HowStart(ServiceControllerStatus? status, out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return HowStartType.NoWay; }

            if (status == ServiceControllerStatus.Paused)
            {
                try
                {
                    if (!serviceController.CanPauseAndContinue)
                    {
                        ex = new Exception("служба не может быть запущена посколько была приостановлена и не поддерживает возобновление работы");
                        return HowStartType.NoWay;
                    }
                }
                catch (Exception except)
                {
                    ex = except;
                    return HowStartType.NoWay;
                }

                ex = null;
                return HowStartType.Continue;

            }
            else if (status == ServiceControllerStatus.Stopped)
            {
                ex = null;
                return HowStartType.Start;
            }

            ex = new Exception(string.Format("служба не может начать работу, поскольку находится в состоянии {0}", status));
            return HowStartType.NoWay;
        }

        protected virtual bool CanStart(out Exception ex)
        {
            var status = GetStatus(out ex);
            if (status == null) return false;

            return HowStart(status, out ex) != HowStartType.NoWay;
        }

        public virtual bool Start(out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return false; }

            Exception exception;
            var status = GetStatus(out exception);
            if (status == null)
            {
                ex = exception;
                return false;
            }

            var howStart = HowStart(status, out exception);
            if (howStart == HowStartType.NoWay)
            {
                ex = exception;
                return false;
            }

            try
            {
                if (howStart == HowStartType.Start)
                    serviceController.Start();
                else if (howStart == HowStartType.Continue)
                    serviceController.Continue();
            }
            catch (Exception except)
            {
                ex = except;
                return false;
            }

            ex = null;
            return true;
        }

        public virtual bool ExecuteCommand(int command, out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null)
            {
                ex = new Exception(ExNotAttachMessage);
                return false;
            }

            Exception exception;
            var status = GetStatus(out exception);
            if (status == null)
            {
                ex = exception;
                return false;
            }

            if (status != ServiceControllerStatus.Running)
            {
                ex = new Exception("не возможно передать команду службе поскольку служба не была запущена");
                return false;
            }

            try
            {
                serviceController.ExecuteCommand(command);
            }
            catch (Exception except)
            {
                ex = except;
                return false;
            }

            ex = null;
            return true;
        }

        public virtual void Dispose()
        {
            var serviceController = _serviceController;
            if (serviceController != null)
            {
                serviceController.Close();
                serviceController.Dispose();
            }

            _serviceController = null;
        }
    }
}