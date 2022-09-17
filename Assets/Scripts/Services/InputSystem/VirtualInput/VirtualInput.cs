using System;
using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public abstract class VirtualInput
    {
        protected static Dictionary<string, KeyCode> keyCodes = new Dictionary<string, KeyCode>();
        
        public abstract float GetAxis(InputMapping input);

        public abstract float GetAxisRaw(InputMapping input);

        public abstract bool GetButton(InputMapping input);

        public abstract bool GetButtonDown(InputMapping input);

        public abstract bool GetButtonUp(InputMapping input);

        public abstract void SetAxis(string name, float value);

        public abstract void SetButtonDown(string name);

        public abstract void SetButtonUp(string name);

        public abstract string GetButtonMapping(InputMapping input);

        protected KeyCode ConvertToKeyCode(string name)
        {
            if (keyCodes.TryGetValue(name, out var keyCode))
            {
                return keyCode;
            }

            if (Enum.TryParse(typeof(KeyCode), name, out var keyCodeObj))
            {
                keyCode = (KeyCode)keyCodeObj;
                keyCodes.Add(name, keyCode);
            }

            return keyCode;
        }
    }
}