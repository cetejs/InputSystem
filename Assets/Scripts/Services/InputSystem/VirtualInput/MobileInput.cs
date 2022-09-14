using System.Collections.Generic;

namespace UD.Services.InputSystem
{
    public class MobileInput : VirtualInput
    {
        private Dictionary<string, VirtualAxis> virtualAxes = new Dictionary<string, VirtualAxis>(16);
        private Dictionary<string, VirtualButton> virtualButtons = new Dictionary<string, VirtualButton>(32);

        public override float GetAxis(InputMapping input)
        {
            return GetVirtualAxis(input.mobile).GetAxis();
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return GetVirtualAxis(input.mobile).GetAxisRaw();
        }

        public override bool GetButton(InputMapping input)
        {
            return GetVirtualButton(input.mobile).GetButton();
        }

        public override bool GetButtonDown(InputMapping input)
        {
            return GetVirtualButton(input.mobile).GetButtonDown();
        }

        public override bool GetButtonUp(InputMapping input)
        {
            return GetVirtualButton(input.mobile).GetButtonUp();
        }

        public override void SetAxis(string name, float value)
        {
            GetVirtualAxis(name).Update(value);
        }

        public override void SetButtonDown(string name)
        {
            GetVirtualButton(name).Pressed();
        }

        public override void SetButtonUp(string name)
        {
            GetVirtualButton(name).Released();
        }

        private VirtualAxis GetVirtualAxis(string name)
        {
            if (!virtualAxes.TryGetValue(name, out var axis))
            {
                virtualAxes.Add(name, new VirtualAxis(name));
            }

            return axis;
        }

        private VirtualButton GetVirtualButton(string name)
        {
            if (!virtualButtons.TryGetValue(name, out var button))
            {
                virtualButtons.Add(name, new VirtualButton(name));
            }

            return button;
        }
    }
}