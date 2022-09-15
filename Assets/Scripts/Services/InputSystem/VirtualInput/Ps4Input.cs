using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class Ps4Input : JoystickInput
    {
        private Dictionary<string, JoystickMapping> ps4Mapping = new Dictionary<string, JoystickMapping>(32);

        public override float GetAxis(InputMapping input)
        {
            var ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return 0.0f;
            }

            return GetAxis(ps4.joystick, ps4.isInvert);
        }

        public override float GetAxisRaw(InputMapping input)
        {
            var ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return 0.0f;
            }

            return GetAxis(ps4.joystick, ps4.isInvert);
        }

        public override bool GetButton(InputMapping input)
        {
            var ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return false;
            }

            return GetButton(ps4.joystick, ps4.isAxis);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            var ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return false;
            }

            return GetButtonDown(ps4.joystick, ps4.isAxis);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            var ps4 = GetPs4Mapping(input.ps4);
            if (ps4 == null)
            {
                return false;
            }

            return GetButtonUp(ps4.joystick, ps4.isAxis);
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

        private JoystickMapping GetPs4Mapping(string name)
        {
            CollectPs4Mapping();
            if (ps4Mapping.TryGetValue(name, out var ps4))
            {
                return ps4;
            }

            Debug.LogError($"Ps4Mapping is not exist key {name}");
            return null;
        }

        private void CollectPs4Mapping()
        {
            if (ps4Mapping.Count > 0)
            {
                return;
            }

            CollectJoystickMapping();
            foreach (var mapping in joystickMappings.Values)
            {
                if (string.IsNullOrEmpty(mapping.ps4))
                {
                    continue;
                }

                ps4Mapping.Add(mapping.ps4, mapping);
            }
        }
    }
}