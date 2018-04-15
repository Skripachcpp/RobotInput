using System;
using System.Security.Principal;

namespace WorkingTools.WinAPI
{
    public static class Acces
    {
        public static bool IsAdmin()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();

            if (windowsIdentity == null)
                throw new NullReferenceException("не удалось получить сведения о пользователе от имени которого запущено приложение");

            var windowsPrincipal = new WindowsPrincipal(windowsIdentity);

            bool isAdmin = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            return isAdmin;
        }
    }
}
