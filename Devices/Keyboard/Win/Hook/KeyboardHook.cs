using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Devices.Keyboard.Win.Hook
{
    internal interface IKeyboardHookEvents
    {
        event KeyboardHook.KeyboardHookCallback KeyDown;
        event KeyboardHook.KeyboardHookCallback KeyUp;
    }

    /// <summary>
    /// Class for intercepting low level keyboard hooks
    /// </summary>
    internal class KeyboardHook : IKeyboardHookEvents
    {
        private static KeyboardHook _global;
        public static IKeyboardHookEvents Global 
        { 
            get 
            {
                if (_global == null)
                {
                    _global = new KeyboardHook(); 
                    _global.Install(); 
                }

                return _global;
            } 
        }

        #region WinAPI
        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        private KeyboardHookHandler _hookHandler;

        private IntPtr _hookId = IntPtr.Zero;
        /// <summary>
        /// Install low level keyboard hook
        /// </summary>
        public void Install()
        {
            _hookHandler = HookFunc;
            _hookId = SetHook(_hookHandler);
        }

        /// <summary>
        /// Remove low level keyboard hook
        /// </summary>
        public void Uninstall()
        {
            UnhookWindowsHookEx(_hookId);
        }

        
        /// <summary>
        /// Registers hook with Windows API
        /// </summary>
        /// <param name="proc">Callback function</param>
        /// <returns>Hook ID</returns>
        private IntPtr SetHook(KeyboardHookHandler proc)
        {
            const int WH_KEYBOARD_LL = 13;// Installs a hook procedure that monitors messages before the system sends them to the destination window procedure.

            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }

        /// <summary>
        /// Default hook call, which analyses pressed keys
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {   
            if (nCode >= 0)
            {
                int intWParam = wParam.ToInt32();
                
                //подготовка параметров
                var keyCode = Marshal.ReadInt32(lParam);

                var keyChangeArgs = new KeyChangeArgs { KeyCode = keyCode };


                //вызов событий
                if (KeyDown != null && (intWParam == WM_KEYDOWN || intWParam == WM_SYSKEYDOWN))
                    KeyDown(keyChangeArgs);

                if (KeyUp != null && (intWParam == WM_KEYUP || intWParam == WM_SYSKEYUP))
                    KeyUp(keyChangeArgs);


                //если клавишу нужно игнорить
                if (keyChangeArgs.Cancel)
                    return (IntPtr) 1;

                //если клавишу нужно переопределить
                if (keyCode != keyChangeArgs.KeyCode)
                    wParam = new IntPtr(keyChangeArgs.KeyCode);
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        

        /// <summary>
        /// Function that will be called when defined events occur
        /// </summary>
        /// <param name="e"/>
        public delegate void KeyboardHookCallback(KeyChangeArgs e);

        public event KeyboardHookCallback KeyDown;
        public event KeyboardHookCallback KeyUp;

        /// <summary>
        /// Destructor. Unhook current hook
        /// </summary>
        ~KeyboardHook()
        {
            Uninstall();
        }
    }

    public class KeyChangeArgs
    {
        public KeyChangeArgs()
        {
            Cancel = false;
        }

        public int KeyCode { get; set; }
        public bool Cancel { get; set; }
    }
}
