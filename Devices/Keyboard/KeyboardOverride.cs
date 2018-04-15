using System;
using System.Collections.Generic;
using System.Linq;
using WorkingTools.Extensions;

namespace Devices.Keyboard
{
    public class KeyboardOverride
    {
        private readonly KeyboardEvents _keyboardEvents;
        private readonly Dictionary<int, int?> _dictOverrideKeys = new Dictionary<int, int?>();

        public KeyboardOverride(KeyboardEvents keyboardEvents, IEnumerable<KeyboardOverrideKeyRule> overrideKeyRules)
        {
            if (keyboardEvents == null) throw new ArgumentNullException("keyboardEvents");
            if (overrideKeyRules == null) throw new ArgumentNullException("overrideKeyRules");

            overrideKeyRules.Where(item => item != null).ForEach(item => _dictOverrideKeys.Add(item.KeyCodeOld, item.KeyCodeNew));
            keyboardEvents.KeyChange += OverrideKey;

            _keyboardEvents = keyboardEvents;
        }

        private void OverrideKey(object sender, KeyArgs args)
        {
            if (_dictOverrideKeys.ContainsKey(args.KeyCode))
            {
                var newKeyCode = _dictOverrideKeys[args.KeyCode];
                if (newKeyCode == null) args.Cancel = true;
                else args.KeyCode = (int)newKeyCode;
            }
        }

        ~KeyboardOverride()
        {
            if (_keyboardEvents != null) _keyboardEvents.KeyChange -= OverrideKey;
        }
    }

    public class KeyboardOverrideKeyRule
    {
        public int KeyCodeOld { get; set; }
        public int? KeyCodeNew { get; set; }
    }
}
