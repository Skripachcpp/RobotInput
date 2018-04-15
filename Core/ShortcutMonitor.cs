using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Keyboard;
using WorkingTools.Extensions;
using WorkingTools.Parallel;

namespace Core
{
    public class ShortcutMonitor : IDisposable
    {
        private readonly Pool _pool = new Pool(-1);
        private readonly SequenceMachine<int> _sequenceMachine;

        private KeyboardEvents _keyboardEvents;

        public ShortcutMonitor(IEnumerable<Shortcut> shortcuts)
        {
            _sequenceMachine = new SequenceMachine<int>(shortcuts.ToArray());
        }

        public bool Started { get; private set; }

        public bool CancelSignificantKey { get; set; }

        private int? _keyKode = null;
        private void KeyChange(object sender, KeyArgs args)
        {
            if (args.KeyEventType == KeyActType.KeyUp)
            {
                if (CancelSignificantKey && _keyKode != null && _keyKode == args.KeyCode)
                    args.Cancel = true;

                return;
            }

            if (_sequenceMachine.Input(args.KeyCode))
            {
                if (CancelSignificantKey)
                {
                    _keyKode = args.KeyCode;
                    args.Cancel = CancelSignificantKey;
                }

                if (_sequenceMachine.Coincided.Any())
                    _sequenceMachine.Coincided.Cast<Shortcut>().ForEach(shortcut => _pool.Invoke(shortcut.Callback));
            }
        }

        public void Start()
        {
            if (Started) 
                return;
            
            Started = true;

            (_keyboardEvents ?? (_keyboardEvents = KeyboardEvents.Create())).KeyChange += KeyChange;
        }

        public void Stop()
        {
            if (!Started) 
                return;

            Started = false;

            if (_keyboardEvents != null) _keyboardEvents.KeyChange -= KeyChange;
        }


        public void Dispose()
        {
            Stop();

            if (_keyboardEvents != null) _keyboardEvents.Dispose();
        }
    }

    public class Shortcut : SequenceMachine<int>.Sequence
    {
        public Shortcut(int[] symbols, ICallback callback) 
            : base(symbols)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            Callback = callback;
        }

        public ICallback Callback { get; set; }
    }
}