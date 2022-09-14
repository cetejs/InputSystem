namespace UD.Services.InputSystem
{
    public struct VirtualAxis
    {
        private float value;

        public string Name { get; }

        public VirtualAxis(string name)
        {
            Name = name;
            value = 0;
        }

        public void Update(float value)
        {
            this.value = value;
        }

        public float GetAxis()
        {
            return value;
        }

        public float GetAxisRaw()
        {
            if (value > 0)
            {
                return 1;
            }

            if (value < 0)
            {
                return -1;
            }

            return value;
        }
    }
}