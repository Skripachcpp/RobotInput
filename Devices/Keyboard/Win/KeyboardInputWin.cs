using System;
using System.Runtime.InteropServices;

namespace Devices.Keyboard.Win
{
    public class KeyboardInputWin : KeyboardInput
    {
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private void SetKey(int keyCode, bool keyPress)
        {
            keybd_event(Convert.ToByte(keyCode), 0, Convert.ToByte(keyPress ? KEYEVENTF_KEYUP : 0), 0);  // нажать/отпустить клавишу
        }

        public override void KeyDown(int keyCode)
        {
            SetKey(keyCode, false);
        }

        public override void KeyUp(int keyCode)
        {
            SetKey(keyCode, true);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}