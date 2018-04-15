using System;
using System.Linq;
using System.ServiceProcess;

namespace WorkingTools.WinAPI
{
    public class ServiceManagerLite : IDisposable
    {
        private const string ExNotAttachMessage = "����������� ����������� � ������";

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
        /// ������������ � ������
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool Attach(ServiceController service, out Exception ex)
        {
            if (service == null)
            {
                ex = new ArgumentOutOfRangeException("service", service, "����������� ��������� ������");
                return false;
            }

            ex = null;
            _serviceController = service;

            return true;
        }

        /// <summary>
        /// ������������ � ������
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool Attach(string serviceName, out Exception ex)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                ex = new ArgumentOutOfRangeException("serviceName", "��� ������ ����� ��� �����������");
                return false;
            }

            var serviceController = ServiceController.GetServices().FirstOrDefault(service => service.ServiceName == serviceName);

            if (serviceController == null)
            {
                ex = new ArgumentOutOfRangeException("serviceName", serviceName, "������ � �������� ������ �� ���� �������");
                return false;
            }

            return Attach(serviceController, out ex);
        }

        /// <summary>
        /// ����������� �� ������
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
        /// �������� ������ ������
        /// </summary>
        /// <returns>������ ��������� ������ ��� null, � ������ � ������ ���������� �� �������</returns>
        public virtual ServiceControllerStatus? GetStatus(out Exception ex)
        {
            var serviceController = _serviceController;
            if (serviceController == null) { ex = new Exception(ExNotAttachMessage); return null; }

            try
            {
                serviceController.Refresh();
                //������ �� �������� �������� ������ ������, 
                //��������� ������ ���� ������� ����� ����������� � ���,
                //��� �� ������ ��������;
                //� ���� ������ �������� � ����������
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
        /// ������� ����� �������
        /// </summary>
        /// <param name="status">������</param>
        /// <param name="timeSpan">������������ ����� ��������</param>
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
                        ex = new Exception("������ �� ����� ���� �������������� ��������� ������ ����������� �� �������������� �������");
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

            ex = new Exception(string.Format("������ �� ����� ������������� ���� ������, ��������� ��������� � ��������� {0}", status));
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
                        ex = new Exception("������ �� ����� ���� ����������� ��������� ������ ����������� �� �������������� �������");
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

            ex = new Exception(string.Format("������ �� ����� ������������, ��������� ��������� � ��������� {0}", status));
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
                        ex = new Exception("������ �� ����� ���� �������� ��������� ���� �������������� � �� ������������ ������������� ������");
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

            ex = new Exception(string.Format("������ �� ����� ������ ������, ��������� ��������� � ��������� {0}", status));
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
                ex = new Exception("�� �������� �������� ������� ������ ��������� ������ �� ���� ��������");
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