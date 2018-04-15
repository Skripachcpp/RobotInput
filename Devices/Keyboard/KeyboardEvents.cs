using System;
using System.Threading.Tasks;
using Devices.Keyboard.Win;

namespace Devices.Keyboard
{
    public abstract class KeyboardEvents : IDisposable
    {
        public static KeyboardEvents Create()
        {
            return new KeyboardEventsWin();
        }


        public event KeyChangeDelegate KeyChange;
        public event KeyChangeAsyncDelegate KeyChangeAsync;


        protected virtual void OnKeyChange(KeyArgs args)
        {
            if (args == null) throw new ArgumentNullException("args");
            var keyChangeDelegate = KeyChange;
            if (keyChangeDelegate != null) keyChangeDelegate(this, args);

            var keyChangeAsyncDelegate = KeyChangeAsync;
            if (keyChangeAsyncDelegate != null) Task.Factory.StartNew(() => keyChangeAsyncDelegate(this, args));
        }
        
        #region constructions
        public delegate void KeyChangeDelegate(object sender, KeyArgs args);
        public delegate void KeyChangeAsyncDelegate(object sender, KeyArgsAsync args);
        #endregion constructions

        public virtual void Dispose()
        {
        }
    }


    public class KeyArgsAsync
    {
        public int KeyCode { get; set; }
        public KeyActType KeyEventType { get; set; }



        protected bool Equals(KeyArgsAsync other)
        {
            return KeyCode == other.KeyCode && KeyEventType == other.KeyEventType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KeyArgsAsync) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (KeyCode * 397) ^ (int)KeyEventType;
            }
        }
    }

    public class KeyArgs : KeyArgsAsync
    {
        public KeyArgs()
        {
            Cancel = false;
        }

        public bool Cancel { get; set; }
    }
    
    public enum KeyActType { KeyDown, KeyUp }
}
