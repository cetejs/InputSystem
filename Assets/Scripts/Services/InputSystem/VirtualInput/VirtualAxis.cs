namespace UD.Services.InputSystem
{
    public class VirtualAxis
    {
        private string name;
        private float value;

        public VirtualAxis(string name)
        {
            this.name = name;
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