namespace UD.Services.InputSystem
{
    public abstract class VirtualInput
    {
        public abstract float GetAxis(InputMapping input);

        public abstract float GetAxisRaw(InputMapping input);

        public abstract bool GetButton(InputMapping input);

        public abstract bool GetButtonDown(InputMapping input);

        public abstract bool GetButtonUp(InputMapping input);

        public abstract void SetAxis(string name, float value);

        public abstract void SetButtonDown(string name);

        public abstract void SetButtonUp(string name);
    }
}