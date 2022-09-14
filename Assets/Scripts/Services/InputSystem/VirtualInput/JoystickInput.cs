using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class JoystickInput : VirtualInput
    {
        private Dictionary<string, UsedButton> usedButtons = new Dictionary<string, UsedButton>();

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
                if (!GetUsedButton(input.joystick).isUsedDown && Mathf.Abs(GetAxisRaw(input)) > 0.0f)
                {
                    GetUsedButton(input.joystick).isUsedDown = true;
                    return true;
                }
                if (GetUsedButton(input.joystick).isUsedDown && Mathf.Abs(GetAxisRaw(input)) == 0.0f)
                {
                    GetUsedButton(input.joystick).isUsedDown = false;
                }

                return false;
            }

            return Input.GetButtonDown(input.joystick);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            if (input.isAxis)
            {
                if (GetUsedButton(input.joystick).isUsedUp && Mathf.Abs(GetAxisRaw(input)) == 0.0f)
                {
                    GetUsedButton(input.joystick).isUsedUp = false;
                    return true;
                }
                if (!GetUsedButton(input.joystick).isUsedUp && Mathf.Abs(GetAxisRaw(input)) > 0.0f)
                {
                    GetUsedButton(input.joystick).isUsedUp = true;
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

        private UsedButton GetUsedButton(string name)
        {
            if (!usedButtons.TryGetValue(name, out var button))
            {
                button = new UsedButton()
                {
                    name = name
                };
                
                usedButtons.Add(name, button);
            }

            return button;
        }

        private class UsedButton
        {
            public string name;
            public bool isUsedDown;
            public bool isUsedUp;
        }
    }
}