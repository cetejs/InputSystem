using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class XboxInput : JoystickInput
    {
        private InputData inputData;
        private Dictionary<string, JoystickMapping> xboxMappings = new Dictionary<string, JoystickMapping>(32);

        public XboxInput(InputData inputData)
        {
            this.inputData = inputData;
            CollectXboxMapping();
        }

        public override float GetAxis(InputMapping input)
        {
            var xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return 0.0f;
            }

            return GetAxis(xbox.joystick);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            var xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return 0.0f;
            }

            return GetAxis(xbox.joystick);
        }

        public override bool GetButton(InputMapping input)
        {
            var xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return false;
            }

            return GetButton(xbox.joystick, xbox.isAxis);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            var xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return false;
            }

            return GetButtonDown(xbox.joystick, xbox.isAxis);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            var xbox = GetXboxMapping(input.xbox);
            if (xbox == null)
            {
                return false;
            }

            return GetButtonUp(xbox.joystick, xbox.isAxis);
        }

        public override void SetAxis(string name, float value)
        {
        }

        public override void SetButtonDown(string name)
        {
        }

        public override void SetButtonUp(string name)
        {
        }
        
        public override string GetButtonMapping(InputMapping input)
        {
            var xbox = GetXboxMapping(input.xbox);
            if (xbox != null)
            {
                return xbox.joystick;
            }

            return null;
        }

        private JoystickMapping GetXboxMapping(string name)
        {
            if (xboxMappings.TryGetValue(name, out var xbox))
            {
                return xbox;
            }

            Debug.LogError($"XboxMapping is not exist key {name}");
            return null;
        }

        private void CollectXboxMapping()
        {
            inputData.ForeachJoystickMappings(joystick =>
            {
                if (string.IsNullOrEmpty(joystick.xbox))
                {
                    return;
                }

                xboxMappings.Add(joystick.xbox, joystick);
            });
        }
    }
}