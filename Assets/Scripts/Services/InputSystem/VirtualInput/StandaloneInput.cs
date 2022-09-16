using UnityEngine;

namespace UD.Services.InputSystem
{
    public class StandaloneInput : VirtualInput
    {
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
                return Input.GetKeyUp(keyCode);
            }

            return Input.GetButtonUp(input.keyboard);
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
    }
}