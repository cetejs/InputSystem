using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class JoystickInput : VirtualInput
    {
        private Dictionary<string, bool> usedButtons = new Dictionary<string, bool>();

        public override float GetAxis(InputMapping input)
        {
            return Input.GetAxis(input.joystick);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return Input.GetAxisRaw(input.joystick);
        }

        public override bool GetButton(InputMapping input)
        {
            if (input.isAxis)
            {
                return Mathf.Abs(GetAxisRaw(input)) > 0.0f;
            }

            return Input.GetButton(input.joystick);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            if (input.isAxis)
            {
                if (!IsUsedButton(input.joystick) && Mathf.Abs(GetAxisRaw(input)) > 0.0f)
                {
                    SetUsedButton(input.joystick, true);
                    return true;
                }
                if (IsUsedButton(input.joystick) && Mathf.Abs(GetAxisRaw(input)) == 0.0f)
                {
                    SetUsedButton(input.joystick, false);
                }

                return false;
            }

            return Input.GetButtonDown(input.joystick);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            if (input.isAxis)
            {
                if (IsUsedButton(input.joystick) && Mathf.Abs(GetAxisRaw(input)) == 0.0f)
                {
                    SetUsedButton(input.joystick, false);
                    return true;
                }
                if (!IsUsedButton(input.joystick) && Mathf.Abs(GetAxisRaw(input)) > 0.0f)
                {
                    SetUsedButton(input.joystick, true);
                }

                return false;
            }

            return Input.GetButtonUp(input.joystick);
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

        private bool IsUsedButton(string name)
        {
            if (!usedButtons.TryGetValue(name, out var isUsed))
            {
                usedButtons[name] = isUsed = false;
            }

            return isUsed;
        }

        private void SetUsedButton(string name, bool value)
        {
            usedButtons[name] = value;
        }
    }
}