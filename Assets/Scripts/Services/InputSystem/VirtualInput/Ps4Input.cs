using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class Ps4Input : JoystickInput
    {
        private InputData inputData;
        private Dictionary<string, JoystickMapping> ps4Mappings = new Dictionary<string, JoystickMapping>(32);

        public Ps4Input(InputData inputData)
        {
            this.inputData = inputData;
            CollectPs4Mapping();
        }

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
        }

        public override void SetButtonDown(string name)
        {
        }

        public override void SetButtonUp(string name)
        {
        }

        public override string GetButtonMapping(InputMapping input)
        {
            var ps4 = GetPs4Mapping(input.ps4);
            if (ps4 != null)
            {
                return ps4.joystick;
            }

            return null;
        }

        private JoystickMapping GetPs4Mapping(string name)
        {
            if (ps4Mappings.TryGetValue(name, out var ps4))
            {
                return ps4;
            }

            Debug.LogError($"Ps4Mapping is not exist key {name}");
            return null;
        }

        private void CollectPs4Mapping()
        {
            inputData.ForeachJoystickMappings(joystick =>
            {
                if (string.IsNullOrEmpty(joystick.ps4))
                {
                    return;
                }

                ps4Mappings.Add(joystick.ps4, joystick);
            });
        }
    }
}