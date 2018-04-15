using System;
using Devices.Keyboard.Win;

namespace Devices.Keyboard
{
    public abstract class KeyboardInput : IDisposable
    {
        public static KeyboardInput Create()
        {
            return new KeyboardInputWin();
        }

        public abstract void KeyDown(int keyCode);
        public abstract void KeyUp(int keyCode);
        
        public virtual void Dispose()
        {
        }
    }
}