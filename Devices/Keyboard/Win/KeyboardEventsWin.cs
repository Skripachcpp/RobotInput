using System;
using Devices.Keyboard.Win.Hook;
using WorkingTools.Map;


namespace Devices.Keyboard.Win
{
    public class KeyboardEventsWin : KeyboardEvents, IDisposable
    {
        private readonly IKeyboardHookEvents _keyboardHook = KeyboardHook.Global;

        public KeyboardEventsWin()
        {
            _keyboardHook.KeyDown += KeyDown;
            _keyboardHook.KeyUp += KeyUp;
        }

        private void OnKeyChange(KeyChangeArgs args, KeyActType keyActType)
        {
            //туда сюда
            var e = new KeyArgs { KeyEventType = keyActType };
            OnKeyChange(Mapper.Map(args, e));
            Mapper.Map(e, args);
        }

        private void KeyUp(KeyChangeArgs args)
        {
            OnKeyChange(args, KeyActType.KeyUp);
        }

        private void KeyDown(KeyChangeArgs args)
        {
            OnKeyChange(args, KeyActType.KeyDown);
        }

        public override void Dispose()
        {
            base.Dispose();

            _keyboardHook.KeyDown -= KeyDown;
            _keyboardHook.KeyUp -= KeyUp;
        }
    }
}
