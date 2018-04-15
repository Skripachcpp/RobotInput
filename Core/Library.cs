using System;
using System.Collections.Generic;
using Devices.Keyboard;
using WorkingTools.Parallel;

namespace Core
{
    public class Library : IDisposable
    {
        private readonly SequencePlayer _sequencePlayer = new SequencePlayer();
        private readonly SequenceRecorder _sequenceRecorder = new SequenceRecorder();

        public const int RecordStopStartKeyCode = 109;
        public const int RecordPleyKeyCode = 107;

        private ShortcutMonitor _shortcutMonitor;
        private KeyboardEvents _keyboardEvents;


        private static void KeyChangeAsync(object sender, KeyArgsAsync args)
        {
            if (args.KeyEventType == KeyActType.KeyUp)
                return;

            Console.WriteLine(args.KeyCode);
        }

        public void HotKey(bool enabled)
        {
            if (_shortcutMonitor == null)
            {
                var shortcuts = new List<Shortcut>
                {
                    new Shortcut(new[] {RecordStopStartKeyCode}, new Callback(RecordStopStart)),
                    new Shortcut(new[] {RecordPleyKeyCode}, new Callback(RecordPley)),
                };

                _shortcutMonitor = new ShortcutMonitor(shortcuts) { CancelSignificantKey = true };
            }

            if (enabled && !_shortcutMonitor.Started)
            {
                _shortcutMonitor.Start();

                //if (_keyboardEvents == null) _keyboardEvents = KeyboardEvents.Create();
                //_keyboardEvents.KeyChangeAsync += KeyChangeAsync;
            }
            else
            {
                if (_shortcutMonitor != null)
                    _shortcutMonitor.Stop();

                //if (_keyboardEvents != null)
                //    _keyboardEvents.KeyChangeAsync -= KeyChangeAsync;
            }
        }

        public void RecordStopStart()
        {
            if (_sequenceRecorder.Recording) RecordStop();
            else RecordStart();
        }

        public void RecordPley()
        {
            if (_sequencePlayer.Playing)
                return;

            _sequenceRecorder.Stop();
            var macro = _sequenceRecorder.Macro;

            //удалить из макроса горячие клавиши начала и остановки макроса
            var maxIndex = macro.Count - 1;
            for (int index = 0; index <= maxIndex; index++)
            {
                var item = macro[index];
                if (item.KeyCode == RecordStopStartKeyCode)
                {
                    macro.RemoveAt(index);
                    maxIndex--;
                }
                else
                    break;
            }

            for (int index = maxIndex; index > 0; index--)
            {
                var item = macro[index];
                if (item.KeyCode == RecordStopStartKeyCode || item.KeyCode == RecordPleyKeyCode)
                    macro.RemoveAt(index);
                else
                    break;
            }

            _sequencePlayer.Pley(macro);
        }

        public void RecordStop()
        {
            _sequenceRecorder.Stop();

            if (ChangeStatus != null) OnChangeStatus(new ChangeStatusArgs() { Status = MacroMachineStatus.Stop });
        }

        public void RecordStart()
        {
            if (_sequencePlayer.Playing)
                return;

            _sequenceRecorder.Clear();
            _sequenceRecorder.Start();

            if (ChangeStatus != null) OnChangeStatus(new ChangeStatusArgs() { Status = MacroMachineStatus.Start });
        }

        public event EventHandler<ChangeStatusArgs> ChangeStatus;

        private void OnChangeStatus(ChangeStatusArgs e)
        {
            var handler = ChangeStatus;
            if (handler != null) handler(null, e);
        }

        public void Dispose()
        {
            if (_sequenceRecorder != null) _sequenceRecorder.Dispose();
            if (_shortcutMonitor != null) _shortcutMonitor.Dispose();
            if (_keyboardEvents != null) _keyboardEvents.Dispose();
        }
    }

    public class ChangeStatusArgs : EventArgs
    {
        public MacroMachineStatus Status { get; set; }
    }

    public enum MacroMachineStatus
    {
        Stop,
        Start,
    }
}
