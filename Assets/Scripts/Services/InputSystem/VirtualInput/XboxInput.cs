using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class XboxInput : JoystickInput
    {
        private Dictionary<string, JoystickMapping> xboxMapping = new Dictionary<string, JoystickMapping>(32);

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

        private JoystickMapping GetXboxMapping(string name)
        {
            CollectXboxMapping();
            if (xboxMapping.TryGetValue(name, out var xbox))
            {
                return xbox;
            }

            Debug.LogError($"XboxMapping is not exist key {name}");
            return null;
        }

        private void CollectXboxMapping()
        {
            if (xboxMapping.Count > 0)
            {
                return;
            }

            CollectJoystickMapping();
            foreach (var mapping in joystickMappings.Values)
            {
                if (string.IsNullOrEmpty(mapping.xbox))
                {
                    continue;
                }

                xboxMapping.Add(mapping.xbox, mapping);
            }
        }
    }
}