using System;
using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class StandaloneInput : VirtualInput
    {
        private Dictionary<string, KeyCode> keyCodeDict = new Dictionary<string, KeyCode>();

        public override float GetAxis(InputMapping input)
        {
            return Input.GetAxis(input.keyboard);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return Input.GetAxisRaw(input.keyboard);
        }

        public override bool GetButton(InputMapping input)
        {
            var keyCode = ConvertToKeyCode(input.keyboard);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return Input.GetButton(input.keyboard);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            var keyCode = ConvertToKeyCode(input.keyboard);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKeyDown(keyCode);
            }

            return Input.GetButtonDown(input.keyboard);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            var keyCode = ConvertToKeyCode(input.keyboard);
            if (keyCode != KeyCode.None)
            {
                return Input.GetKey(keyCode);
            }

            return Input.GetButtonUp(input.keyboard);
        }

        public override void SetAxis(string name, float value)
        {
            Debug.LogError($"This axis '{name}' is not possible to be called for standalone input");
        }

        public override void SetButtonDown(string name)
        {
            Debug.LogError($"This button down '{name}' is not possible to be called for standalone input");
        }

        public override void SetButtonUp(string name)
        {
            Debug.LogError($"This button up '{name}' is not possible to be called for standalone input");
        }

        private KeyCode ConvertToKeyCode(string name)
        {
            if (keyCodeDict.TryGetValue(name, out var keyCode))
            {
                return keyCode;
            }

            if (Enum.TryParse(typeof(KeyCode), name, out var keyCodeObj))
            {
                keyCode = (KeyCode)keyCodeObj;
                keyCodeDict.Add(name, keyCode);
            }

            return keyCode;
        }
    }
}