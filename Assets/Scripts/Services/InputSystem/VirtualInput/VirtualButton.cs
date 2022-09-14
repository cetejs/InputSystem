using UnityEngine;

namespace UD.Services.InputSystem
{
    public class VirtualButton
    {
        private int lastPressedFrame = -1;
        private int releasedFrame = -1;
        private bool isPressed;

        public string Name { get; }

        public VirtualButton(string name)
        {
            Name = name;
            lastPressedFrame = -1;
            releasedFrame = -1;
        }

        public void Pressed()
        {
            if (!isPressed)
            {
                isPressed = true;
                lastPressedFrame = Time.frameCount;
            }
        }

        public void Released()
        {
            isPressed = false;
            releasedFrame = Time.frameCount;
        }

        public bool GetButton()
        {
            return isPressed;
        }

        public bool GetButtonDown()
        {
            return lastPressedFrame - Time.frameCount == -1;
        }

        public bool GetButtonUp()
        {
            return releasedFrame - Time.frameCount == -1;
        }
    }
}