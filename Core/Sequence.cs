using System;
using System.Collections.Generic;
using System.Threading;
using Devices.Keyboard;

namespace Core
{
    public class KeySequence
    {
        public KeySequence(int keyCode, KeyActType keyAct)
        {
            KeyCode = keyCode;
            KeyAct = keyAct;
        }

        public int KeyCode { get; set; }

        public KeyActType KeyAct { get; set; }
    }


    public class SequenceRecorder : IDisposable
    {
        private readonly KeyboardEvents _keyboardEvents;
        private readonly List<KeySequence> _macro = new List<KeySequence>();

        public SequenceRecorder() : this(KeyboardEvents.Create()) { }

        public SequenceRecorder(KeyboardEvents keyboardEvents)
        {
            if (keyboardEvents == null) throw new ArgumentNullException("keyboardEvents");
            
            _keyboardEvents = keyboardEvents;

            Recording = false;
        }


        public bool Recording { get; private set; }
        public IList<KeySequence> Macro { get { return _macro; } }


        private void KeyboardKeyChange(object sender, KeyArgs args)
        {
            _macro.Add(new KeySequence(args.KeyCode, args.KeyEventType));
        }

        public void Start()
        {
            if (!Recording) _keyboardEvents.KeyChange += KeyboardKeyChange;

            Recording = true;
        }

        public void Stop()
        {
            if (Recording) _keyboardEvents.KeyChange -= KeyboardKeyChange;

            Recording = false;
        }

        public void Clear()
        {
            _macro.Clear();
        }

        public void Dispose()
        {
            _keyboardEvents.KeyChange -= KeyboardKeyChange;
        }
    }

    public class SequencePlayer
    {
        private readonly KeyboardInput _keyboardInput;

        public SequencePlayer() : this(KeyboardInput.Create()) { }

        public SequencePlayer(KeyboardInput keyboardInput)
        {
            if (keyboardInput == null) throw new ArgumentNullException("keyboardInput");

            _keyboardInput = keyboardInput;

            Playing = false;
        }


        public bool Playing { get; private set; }

        public void Pley(IEnumerable<KeySequence> macro)
        {
            Playing = true;

            foreach (var key in macro)
            {
                switch (key.KeyAct)
                {
                    case KeyActType.KeyDown:
                        _keyboardInput.KeyDown(key.KeyCode);
                        break;
                    case KeyActType.KeyUp:
                        _keyboardInput.KeyUp(key.KeyCode);
                        break;   
                }
            }

            Playing = false;
        }
    }
}
